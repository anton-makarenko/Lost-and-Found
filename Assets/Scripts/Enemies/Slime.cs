﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Slime : Enemy
{
    [SerializeField] float speed = 3.5f;
    [SerializeField] float nextWaypointDistance = 1.3f;
    [SerializeField] Transform target;
    SoundManager soundManager;
    Path path;
    int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb2d;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        seeker = GetComponent<Seeker>();
        rb2d = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0, .5f);        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            CancelInvoke();
            return;
        }
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
            return;
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb2d.position).normalized;
        if (direction.x < 0)
            transform.localScale = new Vector3(2, 2, 2);
        else if (direction.x > 0)
            transform.localScale = new Vector3(-2, 2, 2);
        transform.Translate(direction * speed * Time.deltaTime);
        float distance = Vector2.Distance(rb2d.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
            currentWaypoint++;
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb2d.position, (Vector2)target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public override void Die()
    {
        soundManager.PlayClip("Bubble");
        base.Die();
    }
}
