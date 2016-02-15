﻿using System;
using UnityEngine;
using System.Collections;

public class GP_ParticipantResult {
	
	private int _Placing;
	private int _Result;
	private int _VersionCode = 0;
	private string _ParticipantId;

	public GP_ParticipantResult(string participantId, int result, int placing) {
		_ParticipantId = participantId;
		_Result = result;
		_Placing = placing;
	}


	public GP_ParticipantResult(int versionCode, string participantId, int result, int placing) {
		_ParticipantId = participantId;
		_Result = result;
		_Placing = placing;
		_VersionCode = versionCode;
	}

	public GP_ParticipantResult(string[] data, int index ) {
		_ParticipantId = data[index];
		_Placing = Convert.ToInt32(data[index + 1]);
		_Result = Convert.ToInt32(data[index + 2]);
		_VersionCode = Convert.ToInt32(data[index + 3]);
	}




	public int Placing {
		get {
			return _Placing;
		}
	}

	public int Result {
		get {
			return _Result;
		}
	}

	public int VersionCode {
		get {
			return _VersionCode;
		}
	}

	public string ParticipantId {
		get {
			return _ParticipantId;
		}
	}
}
