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
    public FixedString64Bytes playerName;   // NetCode���� String�� ����� �� ����. FixedString.. Ÿ������ ����ؾ���

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId && playerName == other.playerName;
    }

    /// <summary>
    /// ��Ʈ��ũ�󿡼� ���Ǵ� ������ ������ type �����ʹ� NetworkSerialize�� ���� ����ȭ�� �ؾ��Ѵ�
    /// </summary>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
    }
}
