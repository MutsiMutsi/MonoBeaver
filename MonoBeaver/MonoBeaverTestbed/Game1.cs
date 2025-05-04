using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BeaverTree.Decorators;
using BeaverTree.Enum;
using BeaverTree.Nodes;
using System;
using System.IO;

namespace MonoBeaverTestbed
{

	public enum SoldierState
	{
		Walking,
		Aiming,
		Reloading,
		Shooting,
	}

	public class Soldier
	{
		public int Ammo = 5;
		public Vector2 Position = new Vector2(16, 16);
		public float Rotation = 0f;

		public DateTime LastGunShot = DateTime.MinValue;

		public SoldierState State = SoldierState.Walking;

		// Helper function to normalize angles to [-π, π]
		private float NormalizeAngle(float angle)
		{
			while (angle > MathF.PI) angle -= 2 * MathF.PI;
			while (angle <= -MathF.PI) angle += 2 * MathF.PI;
			return angle;
		}

		public bool AimAt(Vector2 target)
		{
			Vector2 dif = target - Position;
			float targetAngle = MathF.Atan2(dif.Y, dif.X);

			// Normalize both angles to [-π, π]
			float currentAngle = NormalizeAngle(Rotation);
			targetAngle = NormalizeAngle(targetAngle);

			float angleDiff = NormalizeAngle(targetAngle - currentAngle);

			// If we're already facing the target (within a small threshold)
			if (MathF.Abs(angleDiff) < 0.1f)
			{
				return true;
			}

			// Turn in the shortest direction
			Rotation += MathF.Sign(angleDiff) * 0.025f;

			return false;
		}
	}

	public class HasAmmoCheck : BeaverTask
	{
		private readonly Func<float> _getAmmo;

		public HasAmmoCheck(string name, Func<float> getAmmo)
			: base(name)
		{
			_getAmmo = getAmmo;
		}

		protected override NodeStatus OnExecute()
		{
			return _getAmmo() > 0 ? NodeStatus.Success : NodeStatus.Failure;
		}
	}

	public class ReloadTask : BeaverTimeTask
	{
		Soldier _soldier;

		public ReloadTask(Soldier soldier) : base("Reload", 3.0f)
		{
			_soldier = soldier;
		}

		protected override void OnStart()
		{
			Console.WriteLine("Reload started");
			// Play reload animation, sound, etc.
			_soldier.State = SoldierState.Reloading;
		}

		protected override NodeStatus OnFinish()
		{
			Console.WriteLine("Reload complete");
			// Add ammo, change state, etc.
			_soldier.Ammo += 5;

			return NodeStatus.Success;
		}
	}

	public class Game1 : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private Texture2D _nixel;

		private BeaverSelector tree;

		private Vector2 mousePosition { get; set; }

		public Soldier soldier = new Soldier();

		private FontSystem _fontSystem;

		private BeaverDebugger _beaverDebugger;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_fontSystem = new FontSystem();
			_fontSystem.AddFont(TitleContainer.OpenStream(Path.Combine(Content.RootDirectory, "calibri.ttf")));

			_nixel = new Texture2D(GraphicsDevice, 1, 1);
			_nixel.SetData([Color.White]);

			tree = new BeaverSelector("Root")
			{
				new BeaverSequence("Reload")
				{
					new BeaverInverter("Inverter", new HasAmmoCheck("Has Ammo Check", ()=> {return soldier.Ammo; })),
					new ReloadTask(soldier)
				},
				new BeaverSequence("Find mouse")
				{
					new BeaverExpressionCondition("Out of range",
						() => Vector2.Distance(soldier.Position, mousePosition) > 128f),
					new BeaverAction("Go to mouse", ()=>{
						var dir = mousePosition - soldier.Position;
						dir.Normalize();
						soldier.Position += dir;

						soldier.State = SoldierState.Walking;
					})
				},
				new BeaverSequence("Combat")
				{
					new HasAmmoCheck("Has Ammo Check", ()=>{return soldier.Ammo; }),
					new BeaverExpressionCondition("Is Recovered from shooting",
						() => soldier.LastGunShot.AddSeconds(1) < DateTime.UtcNow),
					new BeaverAction("Aim", () => {
						soldier.State = SoldierState.Aiming;
						return soldier.AimAt(mousePosition) ? NodeStatus.Success : NodeStatus.Running;
					}),
					new BeaverAction("Fire", () => {
						soldier.Ammo--;
						soldier.State = SoldierState.Shooting;
						soldier.LastGunShot = DateTime.UtcNow;
					})
				}
			};


			_beaverDebugger = new BeaverDebugger(_spriteBatch, _fontSystem, tree, new Vector2(256, 16), 16);

		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			mousePosition = Mouse.GetState().Position.ToVector2();
			BeaverTree.BeaverTree.ElapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			tree.Execute();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();
			//Body
			_spriteBatch.Draw(_nixel, new Rectangle(soldier.Position.ToPoint() - new Point(8, 8), new(16, 16)), Color.White);

			//Gun
			_spriteBatch.Draw(_nixel, new Rectangle(soldier.Position.ToPoint(), new(4, 16)), null, Color.Red,
				soldier.Rotation - MathF.PI / 2, new Vector2(0, 0), SpriteEffects.None, 0f);

			//Ammo Count
			var ammoStr = $"Ammo: {soldier.Ammo}";
			var ammoFont = _fontSystem.GetFont(16);
			var ammoStrSize = ammoFont.MeasureString(ammoStr);

			_spriteBatch.DrawString(ammoFont, ammoStr, soldier.Position - new Vector2(0,32), Color.White, origin: ammoStrSize / 2, effect: FontSystemEffect.Stroked, effectAmount: 1);

			var stateStr = Enum.GetName(typeof(SoldierState), soldier.State);
			var stateStrSize = ammoFont.MeasureString(stateStr);

			_spriteBatch.DrawString(ammoFont, stateStr, soldier.Position - new Vector2(0, 32) + ammoStrSize * new Vector2(0, 1), Color.White, origin: stateStrSize / 2, effect: FontSystemEffect.Stroked, effectAmount: 1);

			//BeaverDebugger
			_beaverDebugger.Draw();

			_spriteBatch.End();
		}
	}
}
