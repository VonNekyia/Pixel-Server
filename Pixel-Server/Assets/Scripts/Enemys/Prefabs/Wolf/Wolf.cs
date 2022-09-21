using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy {

	private float aggroRadius = 1f;
	private float followRadius = 3f;
	private Collider2D aggroDetection;
	private Collider2D followDetection;
	[SerializeField] public LayerMask targetLayer;
	

	public bool playerDetected { get; internal set;}
	public bool canSee { get; internal set;}
	public bool isAggro { get; internal set;}
	public bool isFollowing { get; internal set;}

	private void Start() {
		isAggro = false;
		isFollowing = false;
	}

	private void Update() {
		aggroDetection = Physics2D.OverlapCircle(transform.position, aggroRadius, targetLayer);
		playerDetected = aggroDetection != null;
		if (playerDetected && !isAggro) {
			isAggro = true;
			isFollowing = true;
		}
		if (isFollowing) {
			followDetection = Physics2D.OverlapCircle(transform.position, followRadius, targetLayer);
			canSee = followDetection != null;
			Debug.Log(canSee);
			if (!canSee) {
				isAggro = false;
				isFollowing = false;
			}
		}
	}

	private void OnDrawGizmos() {
		if (true) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, aggroRadius);
			if (isFollowing) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(transform.position, followRadius);
			}
		}
	}
}

	


