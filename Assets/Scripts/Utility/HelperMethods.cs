using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperMethods : SingletonBase<HelperMethods> 
{
    public Message AddVector3(Vector3 position, Message currentMessage)
    {
        currentMessage.AddShort((short)(position.x * 100));
        currentMessage.AddShort((short)(position.y * 100));
        currentMessage.AddShort((short)(position.z * 100));
        return currentMessage;
    }

	public Message AddShort(float _value, Message currentMessage)
	{
		currentMessage.AddShort((short)(_value * 100));
		return currentMessage;
	}
}
