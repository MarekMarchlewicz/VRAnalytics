using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine;

public class MovieAnalytics : MonoBehaviour 
{
	private enum Mode { Read, Write }

	[SerializeField] private Mode mode;

	[SerializeField] private VideoPlayer player;

	[SerializeField] private Heatmap heatmap;

	[SerializeField] private Transform cameraTransform;

	[SerializeField] private float angleDifferenceToTriggerEvent = 2f;

    private ViewData view;

	private AnalyticsData analyticsData;

	private bool isPlaying = false;

	private Quaternion lastRotation;

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

	private void OnVideoStarted (VideoPlayer video)
	{
		Debug.Log ("Started");

		isPlaying = true;

		if (mode == Mode.Write) 
		{
            analyticsData = DataStorage.LoadFromFIle<AnalyticsData>(StorageMethod.JSON, "ViewAnalyticsData");

            if(analyticsData == null)
                analyticsData = new AnalyticsData ();

            view = new ViewData();
		}
		else 
		{
			analyticsData = DataStorage.LoadFromFIle<AnalyticsData> (StorageMethod.JSON, "ViewAnalyticsData");
            
			//currentPosition = analyticsData.positions [count];
			//nextPosition = analyticsData.positions [count + 1];
		}
	}
		
	private void OnVideoEnded (VideoPlayer video)
	{
		isPlaying = false;

		if (mode == Mode.Write) 
		{
            analyticsData.views.Add(view);

			DataStorage.SaveToFile<AnalyticsData> (analyticsData, StorageMethod.JSON, "ViewAnalyticsData");

			Debug.Log ("Saved");
		}
	}

	private Vector4 currentPosition, nextPosition;

	private void Update()
	{
		if (isPlaying) 
		{
			if (mode == Mode.Write) 
			{
				if (Quaternion.Angle (lastRotation, cameraTransform.rotation) > angleDifferenceToTriggerEvent) {
					Debug.Log ("ANGLE CHANGED");

					Vector4 newPosition = cameraTransform.forward;
					newPosition.w = (float)player.time;
					view.positions.Add (newPosition);

					lastRotation = cameraTransform.rotation;
				}
			} 
			else 
			{
				//if ((float)player.time > nextPosition.w) 
				//{
				//	if (count + 1 < analyticsData.positions.Count) 
				//	{
				//		count++;

				//		currentPosition = analyticsData.positions [count];
				//		nextPosition = analyticsData.positions [count + 1];
				//	}
				//}

				//float lerp = ((float)player.time - currentPosition.w) / (nextPosition.w - currentPosition.w);

				//Vector3 position = Vector3.Lerp (currentPosition, nextPosition, lerp);

				//heatmap.UpdatePosition (position);
			}
		}
	}
}
