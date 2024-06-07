using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailPoint : MonoBehaviour
{
    public CharaSO character;
    public PlayerOV target;

    void FixedUpdate()
    {
        Vector3 s = (target.handleAngle.forward * character.followDistance) * -1f;
        Vector3 v = new Vector3(s.x, target.transform.position.y, s.z);
        this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position+v, character.selfIA.speed * Time.deltaTime);
    }
}
