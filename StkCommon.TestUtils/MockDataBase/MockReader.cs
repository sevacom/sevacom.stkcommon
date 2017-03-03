using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StkCommon.TestUtils.MockDataBase
{
	public partial class MockDb
	{
		private partial class MockCommand
		{
			/// <summary>
			/// IDataReader
			/// </summary>
			private class MockReader : IDataReader
			{
				private readonly MockReaderData _data;
				private int _rowIndex = -1;
				private int _resultIndex;
				private readonly MockCommandData _cmd;

				public MockReader(MockCommandData data)
				{
					_cmd = data;
					_data = data.ReaderResult;
				}

				public void Dispose()
				{
				}

				public string GetName(int i)
				{
					return _data.Results[_resultIndex].Names[i];
				}

				public string GetDataTypeName(int i)
				{
					return GetFieldType(i).Name;
				}

				public Type GetFieldType(int i)
				{
					var type = _data.Results[_resultIndex].Types[i];
					if (null == type)
					{
						var o = _data.Results[_resultIndex].Values.First()[i];
						if (null == o)
						{
							throw new ArgumentException();
						}
						type = o.GetType();
						_data.Results[_resultIndex].Types[i] = type;
					}
					return type;
				}

				public object GetValue(int i)
				{
					return _data.Results[_resultIndex].Values[_rowIndex][i];
				}

				public int GetValues(object[] values)
				{
					_data.Results[_resultIndex].Values[_rowIndex].CopyTo(values, 0);
					return Math.Min(_data.Results[_resultIndex].Values[_rowIndex].Length, values.Length);
				}

				public int GetOrdinal(string name)
				{
					return _data.Results[_resultIndex].Names.FindIndex(n => n == name);
				}

				public bool GetBoolean(int i)
				{
					return Convert.ToBoolean(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public byte GetByte(int i)
				{
					return Convert.ToByte(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
				{
					var value = _data.Results[_resultIndex].Values[_rowIndex][i];

					var bValue = value as IEnumerable<byte>;
					if (null != bValue)
					{
						var array = bValue.ToArray();
						return ArrayCopy(array, fieldOffset, buffer, bufferoffset, length);
					}

					var cValue = value as IEnumerable<char>;
					if (null != cValue)
					{
						var array = cValue.Select(c => (byte)c).ToArray();
						return ArrayCopy(array, fieldOffset, buffer, bufferoffset, length);
					}

					throw new NotSupportedException();
				}

				public char GetChar(int i)
				{
					return Convert.ToChar(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
				{
					var value = _data.Results[_resultIndex].Values[_rowIndex][i];

					var cValue = value as IEnumerable<char>;
					if (null != cValue)
					{
						var array = cValue.ToArray();
						return ArrayCopy(array, fieldoffset, buffer, bufferoffset, length);
					}

					var bValue = value as IEnumerable<byte>;
					if (null != bValue)
					{
						var array = bValue.Select(b => (char)b).ToArray();
						return ArrayCopy(array, fieldoffset, buffer, bufferoffset, length);
					}

					throw new NotSupportedException();
				}

				public Guid GetGuid(int i)
				{
					return (Guid)_data.Results[_resultIndex].Values[_rowIndex][i];
				}

				public short GetInt16(int i)
				{
					return Convert.ToInt16(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public int GetInt32(int i)
				{
					return Convert.ToInt32(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public long GetInt64(int i)
				{
					return Convert.ToInt64(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public float GetFloat(int i)
				{
					return Convert.ToSingle(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public double GetDouble(int i)
				{
					return Convert.ToDouble(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public string GetString(int i)
				{
					return Convert.ToString(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public decimal GetDecimal(int i)
				{
					return Convert.ToDecimal(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public DateTime GetDateTime(int i)
				{
					return Convert.ToDateTime(_data.Results[_resultIndex].Values[_rowIndex][i]);
				}

				public IDataReader GetData(int i)
				{
					throw new NotSupportedException();
				}

				public bool IsDBNull(int i)
				{
					return null == _data.Results[_resultIndex].Values[_rowIndex][i];
				}

				public int FieldCount
				{
					get { return _data.Results[_resultIndex].Names.Count; }
				}

				object IDataRecord.this[int i]
				{
					get { return _data.Results[_resultIndex].Values[_rowIndex][i]; }
				}

				object IDataRecord.this[string name]
				{
					get
					{
						var i = _data.Results[_resultIndex].Names.IndexOf(name);
						return ((IDataRecord)this)[i];
					}
				}

				public void Close()
				{
					IsClosed = true;
				}

				public DataTable GetSchemaTable()
				{
					var res = new DataTable();

					var data = _data.Results[_resultIndex];
					res.Columns.AddRange(data.Names.Select((n, i) => new DataColumn(n, data.Types[i])).ToArray());

					return res;
				}

				public bool NextResult()
				{
					_rowIndex = -1;
					_resultIndex += 1;
					return _data.Results.Count > _resultIndex;
				}

				public bool Read()
				{
					_cmd.IsUsing = true;
					_rowIndex++;
					return _data.Results[_resultIndex].Values.Count > _rowIndex;
				}

				public int Depth
				{
					get { throw new NotSupportedException(); }
				}

				public bool IsClosed { get; private set; }

				public int RecordsAffected
				{
					get { return _rowIndex + 1; }
				}

				private static long ArrayCopy(Array array, long fieldOffset, Array buffer, int bufferoffset, int length)
				{
					var res = Math.Min(array.Length - fieldOffset, length - bufferoffset);
					Array.Copy(array, fieldOffset, buffer, bufferoffset, res);
					return res;
				}
			}
		}
	}
}