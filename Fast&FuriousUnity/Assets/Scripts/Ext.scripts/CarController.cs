using UnityEngine;

public class CarController : MonoBehaviour {
  public float speed = 0f;
  public float steering = 0f;
  public float maxSpeed = 15f;
  public float turnSpeed = 60f;

  void Update() {
    // Move along axis 90° right of forward: -transform.right so gas = forward, brake = back
    transform.Translate(-transform.right * speed * maxSpeed * Time.deltaTime, Space.World);
    transform.Rotate(Vector3.up * steering * turnSpeed * Time.deltaTime);
  }
}
