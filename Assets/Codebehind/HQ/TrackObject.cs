using HQ;
using UnityEngine;


[CreateAssetMenu( fileName = "New Track",  menuName = "HQ/Track")]
public class TrackObject: ScriptableObject
{
    public int Length;
    public Line[] lines { get; protected set; }
    [HideInInspector]
    public TrackModifier[] Modifier;
    public CarModifier[] Cars;
    public float roadWidth;
    public int segmentLength;
    public float trackHeight;

    private void OnEnable()
    {
        speedTime = 0;
        Construct();
    }

    protected virtual void Construct()
    {
        //Length = Mathf.RoundToInt(trackHeight);
        lines = new Line[Length];

        for (int i = 0; i < Length; i++)
        {
            ref Line line = ref lines[i];
            line.z = i * segmentLength;
            line.w = roadWidth;

            line = ProcessModifier(Modifier, i, line);
        }
    }

    float time;
    float speedTime = 0;
    public void UpdateCars (float delta) 
    {
        speedTime += delta * 10;
        time = 0;

        for (int i = 0; i < Length; i++)
        {
            ref Line line = ref lines[i];
            line = ProcessCars(Cars, i, line);
        }
    }

    Line ProcessModifier (TrackModifier[] modifier, int i, Line line) {
         foreach (var m in modifier)
        {
            if (!m.disabled && m.Segments.InRange(i) && i % m.frequency == 0)
            {
                line.curve += m.curve;
                line.spriteX = m.spriteX;
                line.y += Mathf.Sin(i * m.h) * trackHeight;
                line.sprite = m.sprite ?? line.sprite;
                line.flipX = m.flipX;
            }
        }
        return line;
    }

    Line ProcessCars (CarModifier[] modifier, int i, Line line) {
        var renderCar = false;
         foreach (var m in modifier)
        {
            if (Mathf.RoundToInt(m.position + (speedTime * m.speed) ) == i)
            {
                line.spriteXCar = m.GetLane();
                line.y += Mathf.Sin(i * m.h) * trackHeight;
                line.spriteCar = m.sprite ?? line.spriteCar;
                renderCar = true;
            }
        }
        if (!renderCar) line.spriteCar = null;
        return line;
    }

    public int GetCarUpdatedPos (CarModifier car) {
        return Mathf.RoundToInt(car.position + (speedTime * car.speed));
    }

}

