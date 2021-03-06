﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using MD.PersianDateTime;
using ShoppingCMS.DBConnect;
using ShoppingCMS.Models;


namespace PCMS.OtherClasses
{
    public class ModelFiller
    {
        public static string AppendServername(string url)
        {
            return "https://" + HttpContext.Current.Request.Url.Authority + "/" + url;
        }

        public List<ProductModel> productModels_List(string Query)
        {
            PDBC db = new PDBC("PandaMarketCMS", true);
            db.Connect();

            DataTable dt = db.Select(Query);
            List<ProductModel> list=new List<ProductModel>();
           
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime date = Convert.ToDateTime(dt.Rows[i]["DateCreated"]);
                PersianDateTime persianDateTime = new PersianDateTime(date);
                var model=new ProductModel()
                {
                    Num = i+1,
                    Id = Convert.ToInt32(dt.Rows[i]["id_MProduct"]),
                    Title = dt.Rows[i]["Title"].ToString(),
                    Description = dt.Rows[i]["Description"].ToString(),
                    AddBy = dt.Rows[i]["AddBy"].ToString(),
                    MainPrice = dt.Rows[i]["price"].ToString(),
                    Category = dt.Rows[i]["SubCat"].ToString()+"_"+ dt.Rows[i]["MainCat"].ToString()+"_"+ dt.Rows[i]["type"].ToString(),
                    Date = persianDateTime.ToString(),
                    PicPath =AppendServername(dt.Rows[i]["pic"].ToString())
                };
                if (dt.Rows[i]["IS_AVAILABEL"].ToString()=="1")
                {
                    model.disabled = false;
                }
                else
                {
                    model.disabled = true;
                }

                if (dt.Rows[i]["ISDELETE"].ToString() == "1")
                {
                    model.deleted = true;
                }
                else
                {
                    model.deleted = false;
                }
                list.Add(model);
            }

            return list;
        }


        public string QueryMaker(bool SearchBox,string text="")
        {
            StringBuilder queryBuilder=new StringBuilder();
            queryBuilder.Append(
                " SELECT[id_MProduct],[IS_AVAILABEL],[Description],[DateCreated],[Title],[ISDELETE],(SELECT top 1 [ThumbnailPicAddress]FROM[tbl_Product_Pic]where[tbl_Product_Pic].[id_MProduct]=[tbl_Product].[id_MProduct]) as [pic],(SELECT[PricePerquantity] FROM[tlb_Product_MainProductConnector]WHERE id_MPC=idMPC_WhichTomainPrice) AS price,(SELECT[AdminName]FROM[AdminTbl]where [AdminId]=[id_CreatedByAdmin])as AddBy,(SELECT [PTname]FROM [tbl_Product_Type]where[id_PT]=[id_Type])as [type],(SELECT[SCName]FROM [tbl_Product_SubCategory]where[id_SC]=[id_SubCategory])as SubCat,(SELECT[MCName]FROM [tbl_Product_MainCategory]where[id_MC]=[id_MainCategory])as MainCat FROM [tbl_Product]");

            if (SearchBox)
            {
                queryBuilder.Append("Where Title LIKE N'%");
                queryBuilder.Append(text);
                queryBuilder.Append("%' OR Description LIKE N'%");
                queryBuilder.Append(text);
                queryBuilder.Append("%'");
            }

            queryBuilder.Append(" ORDER BY(DateCreated)DESC");
            return queryBuilder.ToString();
        }

    }
}