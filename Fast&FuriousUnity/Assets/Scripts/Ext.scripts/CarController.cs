using UnityEngine;

public class CarController : MonoBehaviour {
  public float speed = 0f;
  public float steering = 0f;
  public float maxSpeed = 15f;
  public float turnSpeed = 60f;

  void Update() {
    transform.Translate(Vector3.forward * speed * maxSpeed * Time.deltaTime);
    transform.Rotate(Vector3.up * steering * turnSpeed * Time.deltaTime);
  }
}
