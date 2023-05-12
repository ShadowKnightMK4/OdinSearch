using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine
{
    /* Contains class attributes*/


    /// <summary>
    /// When creating tables or converting from a c# class to SQL table, things marked with this are skipped.  Currently used for a couple of properties that are built from the same data
    /// </summary>
    sealed internal class OdinSearchSqlSkipAttrib : Attribute
    {

    }


    /// <summary>
    /// for when more precise record generator is needed
    /// </summary>
    public class OdinSqlPreFab_TypeGen : Attribute
    {


        public OdinSqlPreFab_TypeGen(string BaseType)
        {
            this.BaseType = BaseType;
            MaxArrayLength = 1;
        }


        public OdinSqlPreFab_TypeGen(string BaseType, int MaxArrayLength)
        {
            this.BaseType = BaseType;
            this.MaxArrayLength = MaxArrayLength;
        }

        /// <summary>
        /// Replacement name for the type i.e. is the name of the properity a  reserved word
        /// </summary>
        public string OverrideName;
        /// <summary>
        /// Base type for the property. Ignored if null
        /// </summary>
        public string BaseType;
        /// <summary>
        /// If set to non-zero.  BaseType(MaxArrayLength) sql
        /// </summary>
        public int MaxArrayLength;

        /// <summary>
        /// Set if emitting PRIMARY KEY
        /// </summary>
        public bool IsPrimary;
        /// <summary>
        /// Set if emitting Not 
        /// </summary>
        public bool NotNull;

        public bool IdentitySet { get; private set; } = false;
        private int IdentityStart_ = 1;
        public int IdentityStart
        {
            get
            {
                return IdentityStart_;
            }
            set
            {
                IdentityStart_ = value;
                IdentitySet = true;
            }
        }


        private int IdentityTickup_ = 1;

        public int IdentityTickUp
        {
            get
            {
                return IdentityTickup_;
            }
            set
            {
                IdentityTickup_ = value;
                IdentitySet = true;
            }
        }
    }


}
