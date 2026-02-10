using Godot;
using System;

public partial class ControladorFlecha : Node2D
{
	[Export] public float VelocidadOscilacion = 3.0f; // Velocidad de lado a lado 
	[Export] public float AnguloMaximoGrados = 60.0f; // Cuántos grados se abre (60 izq y 60 der)
	
	private float _tiempoAcumulado = 0.0f;
	private bool _estaApuntando = true;

	public override void _Process(double delta)
	{
		// Solo movemos la flecha si estamos en modo Apuntando
		if (_estaApuntando)
		{
			MoverFlecha((float)delta);
			CheckInputConfirmacion();
		}
	}

	private void MoverFlecha(float delta)
	{
		_tiempoAcumulado += delta * VelocidadOscilacion;

		float ondaSeno = Mathf.Sin(_tiempoAcumulado);

		// conversión grados a radianes
		float anguloMaximoRadianes = Mathf.DegToRad(AnguloMaximoGrados);

		// rotación final
		float rotacionFinal = ondaSeno * anguloMaximoRadianes;
		this.Rotation = rotacionFinal;
	}

	private void CheckInputConfirmacion()
	{
		// detener el movimeinto
		if (Input.IsActionJustPressed("ui_accept"))
		{
			_estaApuntando = false; // Detenemos el movimiento
			GD.Print($"Dirección fijada en: {Mathf.RadToDeg(this.Rotation)} grados");
		
		}
	}
	
	// Función extra
	public void ReiniciarApuntado()
	{
		_estaApuntando = true;
		_tiempoAcumulado = 0;
	}
}
