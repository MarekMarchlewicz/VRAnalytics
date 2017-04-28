using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class MovieAnalytics : MonoBehaviour 
{
	private enum Mode { Read, Write }

	[SerializeField]
    private Mode mode;

	[SerializeField]
    private VideoPlayer player;

	[SerializeField]
    private Heatmap heatmap;

	[SerializeField]
    private Transform cameraTransform;

	[SerializeField]
    private float angleDifferenceToTriggerEvent = 2f;

    [SerializeField]
    private float timeStep = 0.5f;

    private ViewData view;

	private AnalyticsData analyticsData;

	private bool isPlaying = false;

	private Quaternion lastRotation;

    private List<Vector4> positions;

    private void Awake()
	{
		lastRotation = cameraTransform.rotation;

		player.started += OnVideoStarted;
		player.loopPointReached += OnVideoEnded;

		if (player.isPlaying)
			OnVideoStarted (player);
	}

	private void Start()
	{
		player.Play ();
	}

    private void OnDestroy()
    {
        if(isPlaying)
        {
            OnVideoEnded(player);
        }
    }

	private void OnVideoStarted (VideoPlayer video)
	{
		Debug.Log ("Started");

		isPlaying = true;

        analyticsData = DataStorage.LoadFromFIle<AnalyticsData>(StorageMethod.Binary, "ViewAnalyticsData");
        if (analyticsData == null)
        {
            analyticsData = new AnalyticsData();

            Debug.Log("CREATING NEW ANALYTICS DATA");
        }

        Debug.Log(analyticsData.views.Count.ToString() + "  views");

        if (mode == Mode.Write) 
		{
            view = new ViewData();

            InvokeRepeating("TryToGetAnalyticsPoint", 0f, timeStep);
		}
		else 
		{            
            positions = new List<Vector4>(analyticsData.views.Count);

            for (int i = 0; i < analyticsData.views.Count; i++)
            {
                positions.Add(Vector3.zero);
            }
        }
	}
		
	private void OnVideoEnded (VideoPlayer video)
	{
		isPlaying = false;

		if (mode == Mode.Write) 
		{
            analyticsData.views.Add(view);

			DataStorage.SaveToFile<AnalyticsData> (analyticsData, StorageMethod.Binary, "ViewAnalyticsData");

			Debug.Log ("Saved");
		}
        else
        {
            positions.Clear();
            positions = null;
        }

        CancelInvoke("TryToGetAnalyticsPoint");
	}
    
    private void TryToGetAnalyticsPoint()
    {
        if (Quaternion.Angle(lastRotation, cameraTransform.rotation) > angleDifferenceToTriggerEvent)
        {
            Debug.Log("ANGLE CHANGED");

            ViewPosition viewPosition;
            viewPosition.x = cameraTransform.forward.x;
            viewPosition.y = cameraTransform.forward.y;
            viewPosition.z = cameraTransform.forward.z;

            viewPosition.t = (float)player.time;

            view.positions.Add(viewPosition);

            lastRotation = cameraTransform.rotation;
        }
    }

	private void Update()
	{
		if (isPlaying && mode == Mode.Read) 
		{
            UpdateGazePositions();
		}
	}

    private void UpdateGazePositions()
    {
        for (int v = 0; v < analyticsData.views.Count; v++)
        {
            ViewData viewData = analyticsData.views[v];

            int lastPosition = 0;
            int nextPosition = 0;

            for (int p = 0; p < viewData.positions.Count; p++)
            {
                if ((float)player.time > viewData.positions[p].t)
                {
                    lastPosition = p;
                }
                else
                {
                    nextPosition = p;

                    break;
                }
            }

            float lerp = ((float)player.time - viewData.positions[lastPosition].t) / (viewData.positions[nextPosition].t - viewData.positions[lastPosition].t);

            Vector3 currentPosition = Vector3.Lerp(viewData.positions[lastPosition].GetPosition(), viewData.positions[nextPosition].GetPosition(), lerp);

            positions[v] = currentPosition;
        }
        Debug.Log(positions.Count);

        heatmap.UpdatePosition(positions);
    }
}
