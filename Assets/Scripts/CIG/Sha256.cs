using System;
using System.Collections.ObjectModel;

namespace CIG
{
	public class Sha256
	{
		private static readonly uint[] K = new uint[64]
		{
			1116352408u,
			1899447441u,
			3049323471u,
			3921009573u,
			961987163u,
			1508970993u,
			2453635748u,
			2870763221u,
			3624381080u,
			310598401u,
			607225278u,
			1426881987u,
			1925078388u,
			2162078206u,
			2614888103u,
			3248222580u,
			3835390401u,
			4022224774u,
			264347078u,
			604807628u,
			770255983u,
			1249150122u,
			1555081692u,
			1996064986u,
			2554220882u,
			2821834349u,
			2952996808u,
			3210313671u,
			3336571891u,
			3584528711u,
			113926993u,
			338241895u,
			666307205u,
			773529912u,
			1294757372u,
			1396182291u,
			1695183700u,
			1986661051u,
			2177026350u,
			2456956037u,
			2730485921u,
			2820302411u,
			3259730800u,
			3345764771u,
			3516065817u,
			3600352804u,
			4094571909u,
			275423344u,
			430227734u,
			506948616u,
			659060556u,
			883997877u,
			958139571u,
			1322822218u,
			1537002063u,
			1747873779u,
			1955562222u,
			2024104815u,
			2227730452u,
			2361852424u,
			2428436474u,
			2756734187u,
			3204031479u,
			3329325298u
		};

		private readonly byte[] _pendingBlock = new byte[64];

		private readonly uint[] _uintBuffer = new uint[16];

		private readonly uint[] _h = new uint[8]
		{
			1779033703u,
			3144134277u,
			1013904242u,
			2773480762u,
			1359893119u,
			2600822924u,
			528734635u,
			1541459225u
		};

		private uint _pendingBlockOff;

		private ulong _bitsProcessed;

		private bool _closed;

		public void AddData(byte[] data, uint offset, uint len)
		{
			if (_closed)
			{
				throw new InvalidOperationException("Adding data to a closed hasher.");
			}
			if (len == 0)
			{
				return;
			}
			_bitsProcessed += len * 8;
			while (true)
			{
				uint num;
				switch (len)
				{
				case 0u:
					return;
				case 1u:
				case 2u:
				case 3u:
				case 4u:
				case 5u:
				case 6u:
				case 7u:
				case 8u:
				case 9u:
				case 10u:
				case 11u:
				case 12u:
				case 13u:
				case 14u:
				case 15u:
				case 16u:
				case 17u:
				case 18u:
				case 19u:
				case 20u:
				case 21u:
				case 22u:
				case 23u:
				case 24u:
				case 25u:
				case 26u:
				case 27u:
				case 28u:
				case 29u:
				case 30u:
				case 31u:
				case 32u:
				case 33u:
				case 34u:
				case 35u:
				case 36u:
				case 37u:
				case 38u:
				case 39u:
				case 40u:
				case 41u:
				case 42u:
				case 43u:
				case 44u:
				case 45u:
				case 46u:
				case 47u:
				case 48u:
				case 49u:
				case 50u:
				case 51u:
				case 52u:
				case 53u:
				case 54u:
				case 55u:
				case 56u:
				case 57u:
				case 58u:
				case 59u:
				case 60u:
				case 61u:
				case 62u:
				case 63u:
					num = ((_pendingBlockOff + len <= 64) ? len : (64 - _pendingBlockOff));
					break;
				default:
					num = 64 - _pendingBlockOff;
					break;
				}
				Array.Copy(data, offset, _pendingBlock, _pendingBlockOff, num);
				len -= num;
				offset += num;
				_pendingBlockOff += num;
				if (_pendingBlockOff == 64)
				{
					ToUintArray(_pendingBlock, _uintBuffer);
					ProcessBlock(_uintBuffer);
					_pendingBlockOff = 0u;
				}
			}
		}

		public ReadOnlyCollection<byte> GetHash()
		{
			return ToByteArray(GetHashUInt32());
		}

		public ReadOnlyCollection<uint> GetHashUInt32()
		{
			if (!_closed)
			{
				ulong num = _bitsProcessed;
				AddData(new byte[1]
				{
					128
				}, 0u, 1u);
				uint num2 = 64 - _pendingBlockOff;
				if (num2 < 8)
				{
					num2 += 64;
				}
				byte[] array = new byte[num2];
				for (uint num3 = 1u; num3 <= 8; num3++)
				{
					array[num2 - num3] = (byte)num;
					num >>= 8;
				}
				AddData(array, 0u, num2);
				_closed = true;
			}
			return Array.AsReadOnly(_h);
		}

		public static ReadOnlyCollection<byte> HashBytes(params object[] args)
		{
			Sha256 sha = new Sha256();
			int i = 0;
			for (int num = args.Length; i < num; i++)
			{
				object obj = args[i];
				byte[] array;
				if ((array = (obj as byte[])) != null)
				{
					sha.AddData(array, 0u, (uint)array.Length);
					continue;
				}
				ReadOnlyCollection<byte> readOnlyCollection;
				if ((readOnlyCollection = (obj as ReadOnlyCollection<byte>)) != null)
				{
					byte[] array2 = new byte[readOnlyCollection.Count];
					readOnlyCollection.CopyTo(array2, 0);
					sha.AddData(array2, 0u, (uint)array2.Length);
					continue;
				}
				throw new ArgumentException("Cannot add data of type '" + ((obj == null) ? "null" : obj.GetType().Name) + "' to Sha256");
			}
			return sha.GetHash();
		}

		public static byte[] HashToBytes(params object[] args)
		{
			ReadOnlyCollection<byte> readOnlyCollection = HashBytes(args);
			byte[] array = new byte[readOnlyCollection.Count];
			readOnlyCollection.CopyTo(array, 0);
			return array;
		}

		private void ProcessBlock(uint[] m)
		{
			uint[] array = new uint[64];
			for (int i = 0; i < 16; i++)
			{
				array[i] = m[i];
			}
			for (int j = 16; j < 64; j++)
			{
				array[j] = sigma1(array[j - 2]) + array[j - 7] + sigma0(array[j - 15]) + array[j - 16];
			}
			uint num = _h[0];
			uint num2 = _h[1];
			uint num3 = _h[2];
			uint num4 = _h[3];
			uint num5 = _h[4];
			uint num6 = _h[5];
			uint num7 = _h[6];
			uint num8 = _h[7];
			for (int k = 0; k < 64; k++)
			{
				uint num9 = num8 + Sigma1(num5) + Ch(num5, num6, num7) + K[k] + array[k];
				uint num10 = Sigma0(num) + Maj(num, num2, num3);
				num8 = num7;
				num7 = num6;
				num6 = num5;
				num5 = num4 + num9;
				num4 = num3;
				num3 = num2;
				num2 = num;
				num = num9 + num10;
			}
			_h[0] = num + _h[0];
			_h[1] = num2 + _h[1];
			_h[2] = num3 + _h[2];
			_h[3] = num4 + _h[3];
			_h[4] = num5 + _h[4];
			_h[5] = num6 + _h[5];
			_h[6] = num7 + _h[6];
			_h[7] = num8 + _h[7];
		}

		private static uint ROTL(uint x, byte n)
		{
			return (x << (int)n) | (x >> 32 - n);
		}

		private static uint ROTR(uint x, byte n)
		{
			return (x >> (int)n) | (x << 32 - n);
		}

		private static uint Ch(uint x, uint y, uint z)
		{
			return (x & y) ^ (~x & z);
		}

		private static uint Maj(uint x, uint y, uint z)
		{
			return (x & y) ^ (x & z) ^ (y & z);
		}

		private static uint Sigma0(uint x)
		{
			return ROTR(x, 2) ^ ROTR(x, 13) ^ ROTR(x, 22);
		}

		private static uint Sigma1(uint x)
		{
			return ROTR(x, 6) ^ ROTR(x, 11) ^ ROTR(x, 25);
		}

		private static uint sigma0(uint x)
		{
			return ROTR(x, 7) ^ ROTR(x, 18) ^ (x >> 3);
		}

		private static uint sigma1(uint x)
		{
			return ROTR(x, 17) ^ ROTR(x, 19) ^ (x >> 10);
		}

		private static void ToUintArray(byte[] src, uint[] dest)
		{
			uint num = 0u;
			uint num2 = 0u;
			while (num < dest.Length)
			{
				dest[num] = (uint)((src[num2] << 24) | (src[num2 + 1] << 16) | (src[num2 + 2] << 8) | src[num2 + 3]);
				num++;
				num2 += 4;
			}
		}

		private static ReadOnlyCollection<byte> ToByteArray(ReadOnlyCollection<uint> src)
		{
			byte[] array = new byte[src.Count * 4];
			int num = 0;
			int i = 0;
			for (int count = src.Count; i < count; i++)
			{
				array[num++] = (byte)(src[i] >> 24);
				array[num++] = (byte)(src[i] >> 16);
				array[num++] = (byte)(src[i] >> 8);
				array[num++] = (byte)src[i];
			}
			return Array.AsReadOnly(array);
		}
	}
}
