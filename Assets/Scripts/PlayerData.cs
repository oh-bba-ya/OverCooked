using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString64Bytes playerName;   // NetCode에서 String은 사용할 수 없다. FixedString.. 타입으로 사용해야함

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId && playerName == other.playerName;
    }

    /// <summary>
    /// 네트워크상에서 사용되는 유저가 정의한 type 데이터는 NetworkSerialize를 통해 직렬화를 해야한다
    /// </summary>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
    }
}
