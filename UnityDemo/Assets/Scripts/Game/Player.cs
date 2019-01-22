using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text Name;

    public string ID { get; set; }

    public void SetName(string name)
    {
        Name.text = name;
    }

    public void SetType(int type)
    {
        Color[] colors = { Color.blue, Color.cyan, Color.yellow, Color.red, Color.green };
        GetComponent<MeshRenderer>().material.color = colors[type];
    }

    public void MoveTo(float x, float y)
    {
        StopCoroutine("Move");
        Vector3 targetPos = new Vector3(x, 0.5f, y);
        StartCoroutine("Move", targetPos);
    }

    private IEnumerator Move(Vector3 target_pos)
    {
        if (Equals(transform.position, target_pos) == true)
        {
            yield break;
        }

        Vector3 start_pos = transform.position;
        float move_distance = 0;
        float move_speed = 1.0f;
        float move_ratio = 0;
        float total_distance = Vector3.Distance(start_pos, target_pos);

        while (true)
        {
            if (Equals(transform.position, target_pos) == true)
            {
                yield break;
            }

            move_distance += move_speed * Time.deltaTime;
            move_ratio = move_distance / total_distance;
            Vector3 position = Vector3.Lerp(start_pos, target_pos, move_ratio);
            transform.position = position;
            yield return null;
        }
    }
}
