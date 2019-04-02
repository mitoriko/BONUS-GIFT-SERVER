using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class StoreDao
    {


    }

    public class StoreSqls
    {
        public const string SELECT_ORDER_GOODS_LIST_BY_STORE_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_ORDER_GOODS T,T_BUSS_ORDER A,T_BASE_STORE B "
                + "WHERE A.ORDER_CODE = T.ORDER_CODE "
                + "AND B.STORE_CODE = A.STORE_CODE " 
                + "AND STORE_ID = {0} ";
    }
}
