using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Umbala : Enemy {

    public float speed = 0.2f;
	private float aggroRadius = 1f;
	private float followRadius = 3f;
	private float attackRadius = 0.5f;
	private Vector3 offset = new Vector3(0,0.1f,0);
	private Collider2D aggroDetection;
	private Collider2D followDetection;
	private Collider2D attackDetection;
	private Vector3 positionOrigin;
	[SerializeField] public LayerMask targetLayer;
	

	public bool playerDetected { get; internal set;}
	public bool canSee { get; internal set;}
	public bool isAggro { get; internal set;}
	public bool isFollowing { get; internal set;}
	public bool canAttack { get; internal set;}

	private void Start() {
		isAggro = false;
		isFollowing = false;
		positionOrigin = transform.position;
	}

	private void Update() {

		if (!isFollowing) {
			aggroDetection = Physics2D.OverlapCircle(transform.position - offset, aggroRadius, targetLayer);
			playerDetected = aggroDetection != null;
			if (playerDetected && !isAggro) {
				isAggro = true;
				isFollowing = true;
			}
		}

		if (isFollowing) {
			followDetection = Physics2D.OverlapCircle(transform.position - offset, followRadius, targetLayer);
			attackDetection = Physics2D.OverlapCircle(transform.position - offset, attackRadius, targetLayer);
			canAttack = attackDetection != null;
			canSee = followDetection != null;
			//Debug.Log(followDetection.gameObject.transform.position);
			//Debug.Log(gameObject.transform.position);
			
			if (!canSee) {
				isAggro = false;
				isFollowing = false;
			}
			else {
				if (!canAttack) {
					MoveTowardsPosition(this, followDetection.gameObject.transform.position);
				
				}
			}
		}
	}

	private void OnDrawGizmos() {
		if (true) {
			if (!isFollowing) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position - offset, aggroRadius);
			}
			if (isFollowing) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(transform.position - offset, followRadius);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(transform.position - offset, attackRadius);
			}
		}
	}

	private void MoveTowardsPosition(Enemy wolf, Vector3 end) {

		Vector2 wolfPosition = new Vector2(wolf.transform.position.x,wolf.transform.position.y);
		Vector2 playerPosition = new Vector2(end.x,end.y);

		
		Vector2 stärke = new Vector2(Math.Abs(wolfPosition.x - playerPosition.x)*speed,Math.Abs(wolfPosition.y - playerPosition.y)*speed).normalized;
		
		if (wolfPosition.x > playerPosition.x) {
			stärke.x = -stärke.x;
		}
		if (wolfPosition.y > playerPosition.y) {
			stärke.y = -stärke.y;
		}
		
		Vector2 wolfPositionAbs = new Vector2(Math.Abs(wolfPosition.x),Math.Abs(wolfPosition.y));
		Vector2 playerPositionAbs = new Vector2(Math.Abs(playerPosition.x),Math.Abs(playerPosition.y));
		
		if (Math.Round(wolfPositionAbs.x, 1, MidpointRounding.AwayFromZero).Equals(Math.Round(playerPositionAbs.x, 1, MidpointRounding.AwayFromZero))) {
			stärke.x = 0;
		}
		
		if (Math.Round(wolfPositionAbs.y, 1, MidpointRounding.AwayFromZero).Equals(Math.Round(playerPositionAbs.y, 1, MidpointRounding.AwayFromZero))) {
			stärke.y = 0;
		}

		
		wolf.GetComponent<Rigidbody2D>().velocity = stärke;
	}

}
