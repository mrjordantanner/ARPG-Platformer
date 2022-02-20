using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MovementBuffer : MonoBehaviour
    {
        // To be added to Enemies to aid in enemy avoidance
        // Detects collision of buffer trigger with other movement buffers
        // EnemyMovement script uses this info to affect movement behavior

        public float targetPlayerWeight, alignmentWeight, cohesionWeight, separationWeight;
        //public float detectionRange;
        public Vector2 vector;
        public Vector2 velocity = new Vector2();
        public Rigidbody2D rb;
        public float moveSpeed = 3f;

        [HideInInspector]
        public List<MovementBuffer> neighboringBuffers;
        [HideInInspector]
        public bool isColliding;
        //EnemyMovement enemyMovement;
        CircleCollider2D bufferCollider;
        PlayerCharacter player;

        private void Start()
        {
            rb = GetComponentInParent<Rigidbody2D>();
            //enemyMovement = GetComponentInParent<EnemyMovement>();
            bufferCollider = GetComponent<CircleCollider2D>();
            player = PlayerRef.Player;

            velocity = new Vector2();
            neighboringBuffers = new List<MovementBuffer>();
        }

        private void FixedUpdate()
        {
            isColliding = false;
            ComputeVelocity();   // how often to call this?
        }


        public void ComputeVelocity()
        {
            //velocity = Vector2.zero;  //?

            var targetPlayer = ComputePlayerTargeting();

            var alignment = ComputeAlignment();
            var cohesion = ComputeCohesion();
            var separation = ComputeSeparation();

            velocity.x += targetPlayer.x * targetPlayerWeight + alignment.x * alignmentWeight + cohesion.x * cohesionWeight + separation.x * separationWeight;
            velocity.y += targetPlayer.y * targetPlayerWeight + alignment.y * alignmentWeight + cohesion.y * cohesionWeight + separation.y * separationWeight;

            // Pass this to EnemyMovement and incorporate into the general movement scheme there
            rb.AddForce(velocity);
        }

        public Vector2 ComputePlayerTargeting()
        {
            Vector2 v = new Vector2();

            if (player != null)
            {
                v.x += player.transform.position.x;
                v.y += player.transform.position.y;

                v = new Vector2(v.x - transform.position.x, v.y - transform.position.y);
                v.Normalize();
            }
            return v;

        }


        // Adds vector
        public Vector2 ComputeAlignment()
        {
            Vector2 v = new Vector2();
            if (neighboringBuffers.Count == 0) return v;
            foreach (var neighbor in neighboringBuffers)
            {
                v.x += neighbor.vector.x;
                v.y += neighbor.vector.y;

            }

            v.x /= neighboringBuffers.Count;
            v.y /= neighboringBuffers.Count;
            v.Normalize();
            return v;
        }

        // Adds position
        public Vector2 ComputeCohesion()
        {
            Vector2 v = new Vector2();
            if (neighboringBuffers.Count == 0) return v;
            foreach (var neighbor in neighboringBuffers)
            {
                v.x += neighbor.transform.position.x;
                v.y += neighbor.transform.position.y;
                if (neighboringBuffers.Count == 0) return v;
            }

            v.x /= neighboringBuffers.Count;
            v.y /= neighboringBuffers.Count;
            v = new Vector2(v.x - transform.position.x, v.y - transform.position.y);
            v.Normalize();
            return v;
        }


        // Adds distance from the neighbor
        public Vector2 ComputeSeparation()
        {
            Vector2 v = new Vector2();
            if (neighboringBuffers.Count == 0) return v;
            foreach (var neighbor in neighboringBuffers)
            {
                v.x += neighbor.transform.position.x - transform.position.x;
                v.y += neighbor.transform.position.y - transform.position.y;
                if (neighboringBuffers.Count == 0) return v;
            }

            v.x /= neighboringBuffers.Count;
            v.y /= neighboringBuffers.Count;
            v.x *= -1;
            v.y *= -1;
            v = new Vector2(v.x - transform.position.x, v.y - transform.position.y);   //?
            v.Normalize();
            return v;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Add new neighbors to list if they've moved within range
            if (collision.gameObject.CompareTag("Enemy") && !isColliding)
            {
                var buffer = collision.GetComponent<MovementBuffer>();
                if (!neighboringBuffers.Contains(buffer))
                {
                    neighboringBuffers.Add(buffer);
                    isColliding = true;
                }
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Remove neighbors from list if they've moved out of range
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var buffer = collision.GetComponent<MovementBuffer>();
                if (neighboringBuffers.Contains(buffer))
                    neighboringBuffers.Remove(buffer);
                isColliding = true;
            }



        }

    }

}