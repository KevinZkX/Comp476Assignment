using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayState { move, freeze};

public class HumanBehavior : MonoBehaviour{

    static GameState game_state = GameState.Kenimatic;
    PlayState playState = new PlayState();

    float wondTimer;
    float wondTime = 3f;
    float freezeTime = 5f;
    float freezeTimer;


	[SerializeField]Vector3 acceleration;
	Vector3 velocity;
    Vector3 temp_target = Vector3.zero;

    public GameObject target;
    public float field_of_view_angle = 60f;
    float viewRadius;

    [SerializeField] float short_distance = 1f;
    [SerializeField] float maxium_acceleration = 2f;
	[SerializeField] float maxium_velocity_magnitude = 5f;
	[SerializeField] float maixum_angular_velocity_magnitude = 100f;
	[SerializeField] float maixum_angular_acceleration = 5f;
	[SerializeField] float arrive_radius = 0.5f;
	[SerializeField] float slow_down_radius = 3f;
	

	float x_limit = 35f;
	float z_limit = 15f;

    static public void SetState (int stateNumber)
    {
        if (stateNumber == 0)
        {
            game_state = GameState.Kenimatic;
        }
        else if (stateNumber == 1)
        {
            game_state = GameState.Steering;
        }
    }

	Vector3 getAcceleration () {
		return acceleration;
	}

	Vector3 getVelocity () {
		return velocity;
	}

	void Start() {
		acceleration = new Vector3(0, 0, 0);
		velocity = new Vector3(0, 0, 0);
        viewRadius = GetComponent<FieldOfView>().viewRadius;
	}

	void Update() {
        
        if (gameObject.layer == 9 || gameObject.layer == 8)
        {
            if (this.target == null)
            {
                GameObject[] freeze = GameObject.FindGameObjectsWithTag("Freeze");
                if (GetComponent<FieldOfView>().visibleTargets.Count > 0)
                {
                    float distance = 10000f;
                    foreach (Transform go in GetComponent<FieldOfView>().visibleTargets)
                    {
                        float temp_dis = Vector3.Distance(go.position, transform.position);
                        if (temp_dis < distance)
                        {
                            target = go.gameObject;
                            distance = temp_dis;
                        }

                    }
                }
                else if (freeze.Length > 0 && gameObject.layer == 8)
                {
                    foreach (GameObject go in freeze)
                    {
                        target = freeze[0];
                    }
                }
                else
                {
                    Wonder();
                }
            }
            else
            {   
                if (game_state == GameState.Kenimatic)
                {
                    if (gameObject.layer == 9 && (Vector3.Distance(target.transform.position, transform.position) > viewRadius || target.layer == 10))
                    {
                        this.target = null;
                    }
                    else if (target.layer == 10)
                    {
                        Align(target);
                        KenimaticArrive(target);
                        if (Unfreeze())
                        {
                            target = null;
                        }
                    }
                    else
                    {
                        KenimaticImplementation();
                    }
                }
                else if (game_state == GameState.Steering)
                {
                    if (gameObject.layer == 9 && (Vector3.Distance(target.transform.position, transform.position) > viewRadius || target.layer == 10))
                    {
                        this.target = null;
                    }
                    else if (target.layer == 10)
                    {
                        Align(target);
                        SteeringArrive(target.transform.position);
                        if (Unfreeze())
                        {
                            target = null;
                        }
                    }
                    else
                    {
                        SteeringImplementation();
                    }
                }
                
            }
        }
        else if(gameObject.layer == 10)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            target = null;
        }

        FreezeGame();
        
        if (transform.position.x >= 34f && GetComponent<Rigidbody>().velocity.normalized.x >= 0)
            transform.position = new Vector3(-x_limit, 0f, transform.position.z);
        else if (transform.position.x <= -34f && GetComponent<Rigidbody>().velocity.normalized.x <= 0)
            transform.position = new Vector3(x_limit, 0f, transform.position.z);
        else if (transform.position.z >= 14f && GetComponent<Rigidbody>().velocity.normalized.z >= 0)
            transform.position = new Vector3(transform.position.x, 0f, -z_limit);
        else if (transform.position.z <= -14f && GetComponent<Rigidbody>().velocity.normalized.z <= 0)
            transform.position = new Vector3(transform.position.x, 0f, z_limit);

        ChangeViewAngle();
    }

	void KenimaticImplementation () {
		if (gameObject.tag == "Hunter") {
			if (GetComponent<Rigidbody> ().velocity.magnitude < 5f) {
				KenimaticImplementationA ();
			} else {
				KenimaticImplementationB ();
			}
		} else if (gameObject.tag == "Prey") {
			KenimaticImplementationC ();
		}
	}

	void KenimaticImplementationA () {
		if (Vector3.Distance (target.transform.position, transform.position) <= short_distance) {
			KenimaticPursue (target);
		} else if (Vector3.Distance (target.transform.position, transform.position) > short_distance) {
            Align(target);
            KenimaticPursue(target);
		}
	}

	void KenimaticImplementationB () {
		if (Vector3.Angle (target.transform.position - transform.position, transform.forward) < field_of_view_angle) {
			Align (target);
            KenimaticPursue(target);
		} else {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			Align (target);
            KenimaticPursue(target);
		}
	}
		
	void KenimaticImplementationC () {
		if (Vector3.Distance (transform.position, target.transform.position) < short_distance) {
			KenimaticLeave (target);
		} else {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			AlignBack (target);
			KenimaticLeave (target);
		}
	}

	void SteeringImplementation () {
		if (gameObject.layer == 9) {
			if (GetComponent<Rigidbody> ().velocity.magnitude < 5f) {
				SteeringImplementationA ();
			} else {
				SteeringImplementationB ();
			}
		} else if (gameObject.layer == 8) {
			SteeringImplementationC ();
		}
	}

	void SteeringImplementationA () {
		if (Vector3.Distance (target.transform.position, transform.position) <= short_distance) {
			SteeringPurse (target);
		} else if (Vector3.Distance (target.transform.position, transform.position) > short_distance) {
			Align (target);
            SteeringPurse(target);
		}
	}

	void SteeringImplementationB () {
		if (Vector3.Angle (target.transform.position - transform.position, transform.forward) < field_of_view_angle) {
			Align (target);
            SteeringPurse(target);
		} else {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			Align (target);
            SteeringPurse(target);
		}
	}

	void SteeringImplementationC () {
		if (Vector3.Distance (transform.position, target.transform.position) < short_distance) {
			SteeringLeave (target.transform.position);
		} else {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			AlignBack (target);
			SteeringLeave (target.transform.position);
		}
	}

	Vector3 Direction (GameObject target) {
		return (target.transform.position - transform.position).normalized;
	}

	Vector3 Direction (Vector3 target) {
		return (target - transform.position).normalized;
	}

	Vector3 KenimaticVelocity (GameObject target) {
		return maxium_velocity_magnitude * Direction (target);
	}

    Vector3 KenimaticVelocity(Vector3 target)
    {
        return maxium_velocity_magnitude * Direction(target);
    }

    void KenimaticArrive (GameObject target) {
		float dis = Vector3.Distance (target.transform.position, transform.position);
		if (dis < slow_down_radius) {
			float velocity_magnitude = velocity.magnitude;
			velocity_magnitude = Mathf.Min (velocity_magnitude, dis);
			gameObject.GetComponent<Rigidbody> ().velocity = velocity_magnitude * velocity.normalized;
		} else if (dis < arrive_radius) {
			gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		} else {
			gameObject.GetComponent<Rigidbody> ().velocity = KenimaticVelocity (target);
		}
	}

	void KenimaticArrive (Vector3 target) {
		float dis = Vector3.Distance (target, transform.position);
		if (dis < slow_down_radius) {
			float velocity_magnitude = velocity.magnitude;
			velocity_magnitude = Mathf.Min (velocity_magnitude, dis);
			gameObject.GetComponent<Rigidbody> ().velocity = velocity_magnitude * velocity.normalized;
		} else if (dis < arrive_radius) {
			gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		} else {
			gameObject.GetComponent<Rigidbody> ().velocity = KenimaticVelocity (target);
		}
	}

	void  KenimaticLeave (GameObject target) {
		gameObject.GetComponent<Rigidbody> ().velocity = -KenimaticVelocity (target);
	}

	void KenimaticPursue (GameObject target) {
		Vector3 target_velocity = target.GetComponent<HumanBehavior> ().velocity;
		Vector3 target_new_position = target.transform.position + target_velocity * Time.deltaTime;
		Vector3 direction = Direction (target_new_position);
		gameObject.GetComponent<Rigidbody> ().velocity = maxium_velocity_magnitude * direction;
	}

	Vector3 SteeringAcceleration (Vector3 target) {
        if (gameObject.layer == 8)
        {
            return acceleration + maxium_acceleration * (transform.position - target).normalized * Time.deltaTime;
        }
		return acceleration + maxium_acceleration * (target - transform.position).normalized * Time.deltaTime;
	}

	Vector3 SteeringVelocity () {
		Vector3 new_velocity;
		new_velocity = velocity + acceleration * Time.deltaTime;
		float new_velocity_magnitude = new_velocity.magnitude;
		new_velocity_magnitude = (new_velocity_magnitude > maxium_velocity_magnitude) ? maxium_velocity_magnitude : new_velocity_magnitude;
		new_velocity = new_velocity_magnitude * new_velocity.normalized;
		return new_velocity;
	}

	void SteeringArrive (Vector3 target) {
		float distance = (target - transform.position).magnitude;
		if (distance <= slow_down_radius && distance > arrive_radius) {
			float goal_velocity_magnitude = 8f * distance / slow_down_radius;
			goal_velocity_magnitude = Mathf.Min (goal_velocity_magnitude, maxium_velocity_magnitude);
			Vector3 goal_velocity = goal_velocity_magnitude * Direction(target);
			acceleration = (goal_velocity - GetComponent<Rigidbody>().velocity) / Time.deltaTime;
            if (acceleration.magnitude > maxium_acceleration)
            {
                acceleration = maxium_acceleration * acceleration.normalized;
            }
		} else if (distance <= arrive_radius) {
			acceleration = Vector3.zero;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
		} else {
			acceleration = SteeringAcceleration (target);
		}
		gameObject.GetComponent<Rigidbody> ().AddForce (acceleration, ForceMode.Acceleration);
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > maxium_velocity_magnitude)
        {
            GetComponent<Rigidbody>().velocity = maxium_velocity_magnitude * GetComponent<Rigidbody>().velocity.normalized;
        }
	}

	void SteeringLeave (Vector3 target) {
		acceleration = SteeringAcceleration (target);
		gameObject.GetComponent<Rigidbody> ().AddForce (acceleration, ForceMode.Acceleration);
        if (GetComponent<Rigidbody>().velocity.magnitude > maxium_velocity_magnitude)
        {
            GetComponent<Rigidbody>().velocity = maxium_velocity_magnitude * GetComponent<Rigidbody>().velocity.normalized;
        }
	}

	void SteeringPurse (GameObject target) {
		Vector3 target_velocity = target.GetComponent<HumanBehavior> ().getVelocity ();
		Vector3 target_postion = target.transform.position;
		target_postion += target_velocity * Time.deltaTime;
		SteeringArrive (target_postion);
	}

	void Align (GameObject target) {
		Vector3 direciton = Direction (target);
		Quaternion look_where_going = Quaternion.LookRotation (GetComponent<Rigidbody>().velocity.normalized);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, look_where_going, maixum_angular_velocity_magnitude);
	}

	void AlignBack (GameObject target) {
		Vector3 direciton = -Direction (target);
        Quaternion look_where_going = Quaternion.LookRotation(direciton);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look_where_going, maixum_angular_velocity_magnitude);
    }

    void Wonder ()
    {
        if (wondTime < wondTimer || temp_target == Vector3.zero)
        {
            float state = Random.Range(0, 10);
            if (game_state == GameState.Kenimatic)
            {
                if (state < 5)
                {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                else
                {
                    float angle = Random.Range(0, 91);
                    temp_target = new Vector3(transform.forward.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Sin(angle * Mathf.Deg2Rad), 0f, transform.forward.magnitude * Mathf.Pow(Mathf.Cos(angle * Mathf.Deg2Rad), 2));
                    float side = Random.Range(0, 2);
                    if (side < 1)
                    {
                        temp_target = new Vector3(-temp_target.x, 0f, temp_target.z);
                    }
                }
                GetComponent<Rigidbody>().velocity = temp_target.normalized * maxium_velocity_magnitude;
                transform.forward = temp_target.normalized;
            }
            else if (game_state == GameState.Steering)
            {
                Debug.Log("steer in");
                if (state < 5)
                {
                    acceleration = Vector3.zero;
                }
                else
                {
                    float angle = Random.Range(0, 91);
                    temp_target = new Vector3(transform.forward.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Sin(angle * Mathf.Deg2Rad), 0f, transform.forward.magnitude * Mathf.Pow(Mathf.Cos(angle * Mathf.Deg2Rad), 2));
                    float side = Random.Range(0, 2);
                    if (side < 1)
                    {
                        temp_target = new Vector3(-temp_target.x, 0f, temp_target.z);
                    }
                    GetComponent<Rigidbody>().AddForce(temp_target.normalized * maxium_acceleration, ForceMode.Acceleration);
                }
                
                transform.forward = GetComponent<Rigidbody>().velocity.normalized;
            }
            wondTimer = 0;
        }
        else
        {
            wondTimer += Time.deltaTime;
        }
        if (gameObject.layer == 9)
        {
            Debug.Log("temp_target" + temp_target);
        }
    }

    public void ChangePlayState(int state)
    {
        if (state == 1)
        {
            playState = PlayState.freeze;
        }
        else if (state == 0)
        {
            playState = PlayState.move;
        }
    }

    void FreezeGame()
    {
        if (gameObject.layer == 9)
        {
            if (target != null && Vector3.Distance(target.transform.position, transform.position) <= 1f)
            {
                target.layer = 10;
                target.tag = "Freeze";
            }
        }
        else
            return;
    }

    bool Unfreeze ()
    {
        if (target.layer == 10)
        {
            if (Vector3.Distance(target.transform.position, transform.position) <= 3f)
            {
                target.layer = 8;
                target.tag = "Prey";
                return true;
            }
        }
        return false;
    }


    void ChangeViewAngle()
    {
        field_of_view_angle = -18 * GetComponent<Rigidbody>().velocity.magnitude + 180;
        GetComponent<FieldOfView>().viewAngle = field_of_view_angle;
    }
}
