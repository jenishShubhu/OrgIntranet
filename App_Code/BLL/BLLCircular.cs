﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
/// <summary>
/// Summary description for BLLCircular
/// </summary>
public class BLLCircular
{

	public BLLCircular()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int insertCircular(int year, String type, String cirNo,String nep, String eng, String subject, String filename, String username, int checkForReview)
    {
        SqlParameter[] param = new SqlParameter[]
        {
           new SqlParameter("@year",year),
             new SqlParameter("@type",type),
              new SqlParameter("@cirNo",cirNo),
              new SqlParameter("@nep",nep),
              new SqlParameter("@eng",eng),
              new SqlParameter("@subject",subject),
              new SqlParameter("@filename",filename),
              new SqlParameter("@username",username),
              new SqlParameter("@checkForReview", checkForReview)
        };
        return DAO.IUD(param, "insert into Circular(Year, Type, CirNo, NepDate,EngDate, Subject, Filename, Username, CheckForReview) values(@year, @type,@cirNo,@nep,@eng, @subject, @filename,@username,@checkForReview)", CommandType.Text);
    }

    public DataTable GetAllCircular(String type, String year)
    {

        SqlParameter[] param = new SqlParameter[]
        {
            new SqlParameter("@year",year),
            new SqlParameter("@type", type)
        };
        DataTable dt = DAO.GetTable(param, @"
                    SELECT * from Circular where Type = @type and Year = @year order by Id DESC", CommandType.Text);
        return dt;
    }

    public int DeleteCircular(string Id)
    {
        SqlParameter[] param = new SqlParameter[]
        {
            new SqlParameter("@Id",Id)
        };
        return DAO.IUD(param, "deleteCircular", CommandType.StoredProcedure);
        
    } 

    public DataTable GetMainCircularList()
    {
        DataTable dt = DAO.GetTable(null, @"
                    select Year,'Type'= Case when Type='O' then 'Office Instruction Memo'
                    when Type='A' then 'Administration Circular'
                    else 'Unknown' end ,
                    Type as SType,
                    count('Year') 
                    from Circular
                    group by Year, Type", CommandType.Text);

        return dt;
    }

    public string SendMail(string Subject, string Body)
    {
        try
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("jenish.shrestha@nccbank.com.np");
            mailMessage.From = new MailAddress("hrd@nccbank.com.np");
            mailMessage.Subject = Subject;
            mailMessage.Body = Body;
            SmtpClient smtpClient = new SmtpClient("192.168.100.2");
            smtpClient.Send(mailMessage);
            return ("E-mail sent!");
        }
        catch (Exception ex)
        {
            return ("Could not send the e-mail - error: " + ex.Message);
        }
    }

    public DataTable GetCircularReview(string id)
    {
        SqlParameter[] param = new SqlParameter[]
        {
            new SqlParameter("@id",id)
        };

        DataTable dt = DAO.GetTable(param, @"
	 select BranchCode, BranchName from BranchTable where BranchCode Not in (select BranchCode from CircularView where CirNo = (select  distinct CirNo from Circular where Id =  @id))
                   
                    ", CommandType.Text);

        return dt;
    }

    public DataTable CircularDetail(string id)
    {
        SqlParameter[] param = new SqlParameter[]
        {
            new SqlParameter("@id",id)
        };

        DataTable dt = DAO.GetTable(param, @"
                   select * from Circular where Id = @id
                    ", CommandType.Text);

        return dt;
    }

    public int updateCircular(string cirNo, string NDate, string EDate, int id)
    {
        SqlParameter[] param = new SqlParameter[]
        {
           new SqlParameter("@cirNo",cirNo),
           new SqlParameter("@NDate",NDate),
           new SqlParameter("@EDate",EDate),
           new SqlParameter("@id",id),
        };
        return DAO.IUD(param, "update Circular set  CirNo = @cirNo , NepDate =  @NDate ,EngDate =  @EDate where Id = @id", CommandType.Text);
   
    }
}