using DSTMLIB;
using System;

namespace SampleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			string c = Console.ReadLine();
			switch (c)
			{
				case "1":
					teste1();
					break;
				case "2":
					teste2();
					break;
				case "3":
					Cicle();
					break;
				case "4":
					CrossedLocks();
					break;
				default:
					break;
			}
		}

		static void teste1()
		{
			bool res;

			DSTMLib.Init();

			res = DSTMLib.TxBegin();
			PADInt pi_a = DSTMLib.CreatePADInt(0);
			PADInt pi_b = DSTMLib.CreatePADInt(1);
			res = DSTMLib.TxCommit();

			res = DSTMLib.TxBegin();
			pi_a = DSTMLib.AccessPADInt(0);
			pi_b = DSTMLib.AccessPADInt(1);
			pi_a.Write(36);
			pi_b.Write(37);
			Console.WriteLine("a = " + pi_a.Read());
			Console.WriteLine("b = " + pi_b.Read());
			DSTMLib.Status();
			// The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
			res = DSTMLib.Freeze("tcp://localhost:2001/Server");
			res = DSTMLib.Recover("tcp://localhost:2001/Server");
			res = DSTMLib.Fail("tcp://localhost:2002/Server");
			res = DSTMLib.TxCommit();
		}

		static void teste2()
		{
			string args = Console.ReadLine();

			bool res = false;
			PADInt pi_a, pi_b;
			DSTMLib.Init();

			// Create 2 PADInts
			if ((args.Length > 0) && (args[0].Equals("C")))
			{
				try
				{
					res = DSTMLib.TxBegin();
					pi_a = DSTMLib.CreatePADInt(1);
					pi_b = DSTMLib.CreatePADInt(2000000000);
					Console.WriteLine("####################################################################");
					Console.WriteLine("BEFORE create commit. Press enter for commit.");
					Console.WriteLine("####################################################################");
					DSTMLib.Status();
					Console.ReadLine();
					res = DSTMLib.TxCommit();
					Console.WriteLine("####################################################################");
					Console.WriteLine("AFTER create commit returned " + res + " . Press enter for next transaction.");
					Console.WriteLine("####################################################################");
					Console.ReadLine();
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception: " + e.Message);
					Console.WriteLine("####################################################################");
					Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for next transaction.");
					Console.WriteLine("####################################################################");
					Console.ReadLine();
					DSTMLib.TxAbort();
				}
			}

			try
			{
				res = DSTMLib.TxBegin();
				pi_a = DSTMLib.AccessPADInt(1);
				pi_b = DSTMLib.AccessPADInt(2000000000);
				Console.WriteLine("####################################################################");
				Console.WriteLine("Status after AccessPADInt");
				Console.WriteLine("####################################################################");
				DSTMLib.Status();
				if ((args.Length > 0) && ((args[0].Equals("C")) || (args[0].Equals("A"))))
				{
					pi_a.Write(11);
					pi_b.Write(12);
				}
				else
				{
					pi_a.Write(21);
					pi_b.Write(22);
				}
				Console.WriteLine("####################################################################");
				Console.WriteLine("Status after write. Press enter for read.");
				Console.WriteLine("####################################################################");
				DSTMLib.Status();
				Console.WriteLine("1 = " + pi_a.Read());
				Console.WriteLine("2000000000 = " + pi_b.Read());
				Console.WriteLine("####################################################################");
				Console.WriteLine("Status after read. Press enter for commit.");
				Console.WriteLine("####################################################################");
				DSTMLib.Status();
				Console.ReadLine();
				res = DSTMLib.TxCommit();
				Console.WriteLine("####################################################################");
				Console.WriteLine("Status after commit. commit = " + res + "Press enter for verification transaction.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				Console.WriteLine("####################################################################");
				Console.WriteLine("AFTER r/w ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
				DSTMLib.TxAbort();
			}

			try
			{
				res = DSTMLib.TxBegin();
				PADInt pi_c = DSTMLib.AccessPADInt(1);
				PADInt pi_d = DSTMLib.AccessPADInt(2000000000);
				Console.WriteLine("####################################################################");
				Console.WriteLine("1 = " + pi_c.Read());
				Console.WriteLine("2000000000 = " + pi_d.Read());
				Console.WriteLine("Status after verification read. Press enter for commit and exit.");
				Console.WriteLine("####################################################################");
				DSTMLib.Status();
				Console.ReadLine();
				res = DSTMLib.TxCommit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				Console.WriteLine("####################################################################");
				Console.WriteLine("AFTER verification ABORT. Commit returned " + res + " . Press enter for abort and exit.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
				DSTMLib.TxAbort();
			}
		}

		static void Cicle()
		{
			string args = Console.ReadLine();
			bool res = false;
			int aborted = 0, committed = 0;

			DSTMLib.Init();
			try
			{
				if ((args.Length > 0) && (args[0].Equals("C")))
				{
					res = DSTMLib.TxBegin();
					PADInt pi_a = DSTMLib.CreatePADInt(2);
					PADInt pi_b = DSTMLib.CreatePADInt(2000000001);
					PADInt pi_c = DSTMLib.CreatePADInt(1000000000);
					pi_a.Write(0);
					pi_b.Write(0);
					res = DSTMLib.TxCommit();
				}
				Console.WriteLine("####################################################################");
				Console.WriteLine("Finished creating PADInts. Press enter for 300 R/W transaction cycle.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				Console.WriteLine("####################################################################");
				Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
				DSTMLib.TxAbort();
			}
			for (int i = 0; i < 300; i++)
			{
				try
				{
					res = DSTMLib.TxBegin();
					PADInt pi_d = DSTMLib.AccessPADInt(2);
					PADInt pi_e = DSTMLib.AccessPADInt(2000000001);
					PADInt pi_f = DSTMLib.AccessPADInt(1000000000);
					int d = pi_d.Read();
					d++;
					pi_d.Write(d);
					int e = pi_e.Read();
					e++;
					pi_e.Write(e);
					int f = pi_f.Read();
					f++;
					pi_f.Write(f);
					Console.Write(".");
					res = DSTMLib.TxCommit();
					if (res) { committed++; Console.Write("."); }
					else
					{
						aborted++;
						Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception: " + e.Message);
					Console.WriteLine("####################################################################");
					Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
					Console.WriteLine("####################################################################");
					Console.ReadLine();
					DSTMLib.TxAbort();
					aborted++;
				}

			}
			Console.WriteLine("####################################################################");
			Console.WriteLine("committed = " + committed + " ; aborted = " + aborted);
			Console.WriteLine("Status after cycle. Press enter for verification transaction.");
			Console.WriteLine("####################################################################");
			DSTMLib.Status();
			Console.ReadLine();

			try
			{
				res = DSTMLib.TxBegin();
				PADInt pi_g = DSTMLib.AccessPADInt(2);
				PADInt pi_h = DSTMLib.AccessPADInt(2000000001);
				PADInt pi_j = DSTMLib.AccessPADInt(1000000000);
				int g = pi_g.Read();
				int h = pi_h.Read();
				int j = pi_j.Read();
				res = DSTMLib.TxCommit();
				Console.WriteLine("####################################################################");
				Console.WriteLine("2 = " + g);
				Console.WriteLine("2000000001 = " + h);
				Console.WriteLine("1000000000 = " + j);
				Console.WriteLine("Status post verification transaction. Press enter for exit.");
				Console.WriteLine("####################################################################");
				DSTMLib.Status();
				Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				Console.WriteLine("####################################################################");
				Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
				DSTMLib.TxAbort();
			}
		}

		static void CrossedLocks()
		{
			string args = Console.ReadLine();

			bool res = false;
			PADInt pi_a, pi_b;
			DSTMLib.Init();

			if ((args.Length > 0) && (args[0].Equals("C")))
			{
				try
				{
					res = DSTMLib.TxBegin();
					pi_a = DSTMLib.CreatePADInt(1);
					pi_b = DSTMLib.CreatePADInt(2000000000);
					Console.WriteLine("####################################################################");
					Console.WriteLine("BEFORE create commit. Press enter for commit.");
					Console.WriteLine("####################################################################");
					DSTMLib.Status();
					Console.ReadLine();
					res = DSTMLib.TxCommit();
					Console.WriteLine("####################################################################");
					Console.WriteLine("AFTER create commit. commit = " + res + " . Press enter for next transaction.");
					Console.WriteLine("####################################################################");
					Console.ReadLine();
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception: " + e.Message);
					Console.WriteLine("####################################################################");
					Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
					Console.WriteLine("####################################################################");
					Console.ReadLine();
					DSTMLib.TxAbort();
				}

			}

			try
			{
				res = DSTMLib.TxBegin();
				if ((args.Length > 0) && ((args[0].Equals("A")) || (args[0].Equals("C"))))
				{
					pi_b = DSTMLib.AccessPADInt(2000000000);
					pi_b.Write(211);
					Console.WriteLine("####################################################################");
					Console.WriteLine("Status post first op: write. Press enter for second op.");
					Console.WriteLine("####################################################################");
					DSTMLib.Status();
					Console.ReadLine();
					pi_a = DSTMLib.AccessPADInt(1);
					//pi_a.Write(212);
					Console.WriteLine("####################################################################");
					Console.WriteLine("Status post second op: read. uid(1)= " + pi_a.Read() + ". Press enter for commit.");
					Console.WriteLine("####################################################################");
					DSTMLib.Status();
					Console.ReadLine();
				}
				else
				{
					pi_a = DSTMLib.AccessPADInt(1);
					pi_a.Write(221);
					Console.WriteLine("####################################################################");
					Console.WriteLine("Status post first op: write. Press enter for second op.");
					Console.WriteLine("####################################################################");
					DSTMLib.Status();
					Console.ReadLine();
					pi_b = DSTMLib.AccessPADInt(2000000000);
					//pi_b.Write(222);
					Console.WriteLine("####################################################################");
					Console.WriteLine("Status post second op: read. uid(1)= " + pi_b.Read() + ". Press enter for commit.");
					Console.WriteLine("####################################################################");
					DSTMLib.Status();
					Console.ReadLine();
				}
				res = DSTMLib.TxCommit();
				Console.WriteLine("####################################################################");
				Console.WriteLine("commit = " + res + " . Press enter for verification transaction.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				Console.WriteLine("####################################################################");
				Console.WriteLine("AFTER r/w ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
				DSTMLib.TxAbort();
			}

			try
			{
				res = DSTMLib.TxBegin();
				PADInt pi_c = DSTMLib.AccessPADInt(1);
				PADInt pi_d = DSTMLib.AccessPADInt(2000000000);
				Console.WriteLine("0 = " + pi_c.Read());
				Console.WriteLine("2000000000 = " + pi_d.Read());
				Console.WriteLine("####################################################################");
				Console.WriteLine("Status after verification read. Press enter for verification commit.");
				Console.WriteLine("####################################################################");
				DSTMLib.Status();
				res = DSTMLib.TxCommit();
				Console.WriteLine("####################################################################");
				Console.WriteLine("commit = " + res + " . Press enter for exit.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				Console.WriteLine("####################################################################");
				Console.WriteLine("AFTER verification ABORT. Commit returned " + res + " . Press enter for abort and exit.");
				Console.WriteLine("####################################################################");
				Console.ReadLine();
				DSTMLib.TxAbort();
			}
		}
	}
}
