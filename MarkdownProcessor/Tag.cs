using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace MarkdownProcessor
{
	public class Tag
	{
		public int Index;
		public string Type;
		public int Length;

		public Tag(int index, string type)
		{
			Index = index;
			Type = type;
			Length = type.Length;
		}

		public override bool Equals(object obj)
		{
			if (typeof (Tag) != obj.GetType())
				return false;
			var newTag = (Tag) obj;
			return Equals(newTag);
		}

		protected bool Equals(Tag other)
		{
			return Index == other.Index && string.Equals(Type, other.Type) && Length == other.Length;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Index;
				hashCode = (hashCode*397) ^ (Type != null ? Type.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ Length;
				return hashCode;
			}
		}

		public override string ToString()
		{
			return String.Format("{0} {1} {2}", Index, Type, Length);
		}
	}
}
