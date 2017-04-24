using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class SpitController : MonoBehaviour
    {
        public float DespawnDistance = 0.05f;
        public float Speed = 4f;
        public Transform Target;

        public void Update()
        {
            if (Target == null || Target.transform == null)
            {
                Destroy(gameObject);
                return;
            }

            var distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
            if (!(distanceToTarget <= DespawnDistance))
            {
                MoveToTarget();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void MoveToTarget()
        {
            var direction = Target.transform.position - transform.position;

            transform.rotation = transform.position.x > Target.transform.position.x
                ? new Quaternion(0f, 0f, 0f, 0f)
                : new Quaternion(0f, 180f, 0f, 0f);

            if (direction.magnitude >= DespawnDistance)
            {
                var move = direction * Speed * Time.deltaTime;
                transform.position = new Vector3(
                    transform.position.x + move.x,
                    transform.position.y + move.y,
                    transform.position.z
                );
            }
        }
    }
}