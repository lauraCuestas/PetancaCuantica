using Godot;
using System;

public partial class MedidorFuerza : Node2D
{
	// --- REFERENCIAS ---
	[Export] public Node2D FlechaPivote;
	[Export] public TextureProgressBar BarraVisual;
	[Export] public RigidBody2D Pelota;

	// --- CONFIGURACIÓN ---
	[Export] public float VelocidadFlecha = 3.0f;
	[Export] public float AnguloMaximo = 45.0f;
	[Export] public float VelocidadCarga = 150.0f;
	[Export] public float MultiplicadorFuerza = 20.0f;

	// --- CORRECCIÓN DE ÁNGULO ---
	[Export] public float GradosCorreccion = 0.0f; 

	// --- ESTADOS ---
	private enum Estado { Apuntando, CargandoFuerza, Lanzado }
	private Estado _estadoActual = Estado.Apuntando;

	private float _tiempoFlecha = 0.0f;
	private bool _fuerzaSubiendo = true;

	public override void _Ready()
	{
		if (BarraVisual != null)
		{
			BarraVisual.Value = 0;
			BarraVisual.Visible = false;
		}
	}

	public override void _Process(double delta)
	{
		if (_estadoActual != Estado.Lanzado && Input.IsActionJustPressed("ui_accept"))
		{
			AvanzarEstado();
		}

		switch (_estadoActual)
		{
			case Estado.Apuntando:
				MoverFlecha((float)delta);
				break;
			case Estado.CargandoFuerza:
				OscilarBarra((float)delta);
				break;
		}
	}

	private void MoverFlecha(float delta)
	{
		if (FlechaPivote == null) return;
		_tiempoFlecha += delta * VelocidadFlecha;
		float angulo = Mathf.Sin(_tiempoFlecha) * Mathf.DegToRad(AnguloMaximo);
		FlechaPivote.Rotation = angulo;
	}

	private void OscilarBarra(float delta)
	{
		if (BarraVisual == null) return;
		float paso = VelocidadCarga * delta;

		if (_fuerzaSubiendo)
		{
			BarraVisual.Value += paso;
			if (BarraVisual.Value >= BarraVisual.MaxValue)
			{
				BarraVisual.Value = BarraVisual.MaxValue;
				_fuerzaSubiendo = false;
			}
		}
		else
		{
			BarraVisual.Value -= paso;
			if (BarraVisual.Value <= 0)
			{
				BarraVisual.Value = 0;
				_fuerzaSubiendo = true;
			}
		}
	}

	private void AvanzarEstado()
	{
		switch (_estadoActual)
		{
			case Estado.Apuntando:
			
				EfectoReboteFlecha(); 
				
				// cargar fuerza
				_estadoActual = Estado.CargandoFuerza;
				BarraVisual.Visible = true;
				BarraVisual.Value = 0;
				_fuerzaSubiendo = true;
				break;

			case Estado.CargandoFuerza:
				// disparo
				_estadoActual = Estado.Lanzado;
				BarraVisual.Visible = false;
				FlechaPivote.Visible = false;
				EjecutarLanzamiento();
				break;
		}
	}

	// --- Animación Flecha ---
	private void EfectoReboteFlecha()
	{
		if (FlechaPivote == null) return;

		Tween tween = CreateTween();
		tween.TweenProperty(FlechaPivote, "scale", new Vector2(1.2f, 1.2f), 0.1f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(FlechaPivote, "scale", new Vector2(1.0f, 1.0f), 0.1f);
	}

	private void EjecutarLanzamiento()
	{
		if (Pelota == null) return;

		float anguloFinal = FlechaPivote.Rotation + Mathf.DegToRad(GradosCorreccion);

		Vector2 direccion = Vector2.Right.Rotated(anguloFinal);

		float fuerzaFinal = (float)BarraVisual.Value * MultiplicadorFuerza;
		Pelota.ApplyImpulse(direccion * fuerzaFinal);

		GD.Print($"Lanzado con corrección de {GradosCorreccion} grados.");
	}

	public void ReiniciarJuego()
	{
		_estadoActual = Estado.Apuntando;
		BarraVisual.Visible = false;
		FlechaPivote.Visible = true;
		
		// Aseguramos que la escala esté bien 
		FlechaPivote.Scale = new Vector2(1, 1);

		Pelota.LinearVelocity = Vector2.Zero;
		Pelota.AngularVelocity = 0;
		Pelota.Position = new Vector2(500, 300); 
	}
}
