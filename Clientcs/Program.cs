using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static int[] long2doubleInt(long a)
    {
        int a1 = (int)(a & uint.MaxValue);
        int a2 = (int)(a >> 32);
        return new int[] { a1, a2 };
    }
    static bool[] longTobits(long value)
    {
        var array = long2doubleInt(value);
        BitArray b = new BitArray(array);
        bool[] bits = new bool[64];
        b.CopyTo(bits, 0);
        Array.Reverse(bits);
        return bits;
    }
    static long boolstoLong(bool[] bools)
    {
        bool[] temp = new bool[64];
        temp = bools;
        Array.Reverse(temp);
        BitArray bits = new BitArray(temp);
        var array = new int[2];
        bits.CopyTo(array, 0);
        return (uint)array[0] + ((long)(uint)array[1] << 32);
    }
    static bool[] bytestobits(byte val)
    {
        BitArray b = new BitArray(new byte[] { val });
        bool[] bits = new bool[64];
        b.CopyTo(bits, 0);
        Array.Reverse(bits);
        return bits;
    }
    static bool[] append(bool[] op, bool[] l1, bool[] l2, bool[] l3, bool[] answer, bool[] stat, bool[] iden, bool flag, bool flag2)
    {
        bool[] message = new bool[272];
        op.CopyTo(message, 0);
        l1.CopyTo(message, 2);
        l2.CopyTo(message, 66);
        l3.CopyTo(message, 130);
        answer.CopyTo(message, 194);
        stat.CopyTo(message, 258);
        iden.CopyTo(message, 262);
        message[266] = flag;
        message[267] = flag2;
        for(int i=268; i < 272; i++)
        message[i] = false;
        return message;
    }
    static byte[] boolstoBytes(bool[] bools)
    {
        int index = 0;
        int licznik = 0;
        byte[] Bytes = new byte[34];
        for (int i = 0; i < 34; i++)
        {
            bool[] temp = new bool[8];
            for (int j = 0; j < 8; j++)
            {
                temp[j] = bools[index];
                index++;
            }

            Array.Reverse(temp);
            BitArray bits = new BitArray(temp);
            bits.CopyTo(Bytes, licznik);
            licznik += 1;
        }
        return Bytes;
    }
   
    static bool[] byteto8bits(byte val)
    {
        BitArray b = new BitArray(new byte[] { val });
        bool[] bits = new bool[8];
        b.CopyTo(bits, 0);
        Array.Reverse(bits);
        return bits;
    }
    static bool[] bytesToBools(byte[] bytes)
    {
        bool[] arr2 = new bool[272];
        int index = 0;
        for (int i = 0; i < bytes.Length; i++)
        {
            bool[] temp;
            temp = byteto8bits(bytes[i]);
            temp.CopyTo(arr2, index);
            index += 8;
        }
        return arr2;
    }
   static long id = 0;
    static void  art(UdpClient client, IPEndPoint ep)
    {
        //267 bitow + 5 bit uzupelniajacych
        bool[] operation = new bool[2];
        bool[] number1;
        bool[] number2;
        bool[] number3;
        bool[] ans = new bool[64];
        for (int i = 0; i < 64; i++) ans[i] = false;
        bool[] status = { false,false,false,false };
        bool[] identifier = new bool[4];
        if (id == 0)
        {
            for (int i = 0; i < 4; i++) identifier[i] = false;
        }
        else
        {
            bool[] temp = new bool[64];
            temp = longTobits(id);
            for (int i = 0; i < 4; i++)
                identifier[i] = temp[i + 60];
        }
        bool ifLastNumber = false;
        bool operationOrAddition = false;
        int number1_, number2_, number3_;
        string option;

        Console.WriteLine("[1]  mnozenie");
        Console.WriteLine("[2]  dodawanie");
        Console.WriteLine("[3]  srednia artmeteyczna");
        Console.WriteLine("[4]  srednia geometryczna");
        Console.Write("Podaj numer operacji ktora chcesz wykonac: ");
        option = Console.ReadLine();
        Console.WriteLine("==============================");
        Console.Write("Podaj liczbe nr 1: ");
        number1_ = int.Parse(Console.ReadLine());
        Console.Write("Podaj liczbe nr 2: ");
        number2_ = int.Parse(Console.ReadLine());
        Console.Write("Podaj liczbe nr 3: ");
        number3_ = int.Parse(Console.ReadLine());
        if (option == "1")
        {
            bool[] operac = { false, false };
            operation = operac;
        }
        else if(option == "2")
        {
            bool[] operac = { false, true };
            operation = operac;
        }
        else if(option =="3")
        {
            bool[] operac = { true, false };
            operation = operac;
        }
        else if(option == "4")
        {
            bool[] operac = { true, true };
            operation = operac;

        }
        number1 = longTobits(number1_);
        number2 = longTobits(number2_);
        number3 = longTobits(number3_);
        bool[] Message = append(operation, number1, number2, number3, ans, status, identifier, ifLastNumber,
           operationOrAddition );
        byte[] sendbuf = boolstoBytes(Message);
        client.Send(sendbuf, sendbuf.Length,ep);
        Console.WriteLine("Dane zostaly wyslane!");
      
            while (true)
            {

                byte[] data = client.Receive(ref ep);
                if (data.Length != 0)
                {
                    bool[] answer = bytesToBools(data);
                    if(answer[258] == false && answer[259] == false && answer[260] == false && answer[261] == false )
                    {
                        bool[] result = new bool[64];
                        for(int i = 194; i < 258; i++)
                        {
                            result[i - 194] = answer[i];
                        }
                        long finalResult = boolstoLong(result);
                        Console.WriteLine("Wynik dzialania: " + finalResult);
                    bool[] id_ = new bool[4];
                    for (int i = 262; i < 266; i++)
                    {
                        id_[i - 262] = answer[i];
                    }
                    id = boolstoLong(id_);
                    break;
                    }
                else if (answer[258] == false && answer[259] == false && answer[260] == true && answer[261] == true)
                {
                    Console.WriteLine("Przynajmniej jeden z argumentow jest niepoprawny!");
                    break;
                }
                else if (answer[258] == false && answer[259] == false && answer[260] == false && answer[261] == true)
                {
                    Console.WriteLine("Uzyskana wartosc wykracza poza zakres!");
                    break;
                }
            }
            }
        

    }
    static void addition(UdpClient client, IPEndPoint ep)
    {
        string option;
        do
        {
            bool[] operation = { false, false };
        bool[] number1;
        bool[] number2 = new bool[64];
        for (int i = 0; i < 64; i++) number2[i] = false;
        bool[] number3 = new bool[64];
        for (int i = 0; i < 64; i++) number3[i] = false;
        bool[] ans = new bool[64];
        for (int i = 0; i < 64; i++) ans[i] = false;
        bool[] status = { false, false, false, false };
        bool[] identifier = new bool[4];
        if (id == 0)
        {
            for (int i = 0; i < 4; i++) identifier[i] = false;
        }
        else
        {
            bool[] temp = new bool[64];
            temp = longTobits(id);
            for (int i = 0; i < 4; i++)
                identifier[i] = temp[i + 60];
        }
        bool ifLastNumber = false;
        bool operationOrAddition = true;
        int number;
        

       
            Console.WriteLine("[1]  Wyslij kolejna liczbe calkowita");
            Console.WriteLine("[2]  Zakoncz");
            Console.Write("Podaj numer operacji ktora chcesz wykonac: ");
            option = Console.ReadLine();
            Console.WriteLine("==============================");
            if (option == "1")
            {
                Console.Write("Podaj liczbe do wyslania: ");
                number = int.Parse(Console.ReadLine());
                number1 = longTobits(number);
            }
            else
            {
                number1 = new bool[64];
                for (int i = 0; i < 64; i++) number1[i] = false;
                ifLastNumber = true;
            }
            bool[] Message = append(operation, number1, number2, number3, ans, status, identifier, ifLastNumber, operationOrAddition);
            byte[] sendbuf = boolstoBytes(Message);
            client.Send(sendbuf, sendbuf.Length, ep);
            Console.WriteLine("Dane zostaly wyslane!");


            while (true)
            {

                byte[] data = client.Receive(ref ep);
                if (data.Length != 0)
                {
                    bool[] answer = bytesToBools(data);
                    if (answer[258] == false && answer[259] == false && answer[260] == false && answer[261] == false)
                    {

                        if (answer[266] == false)
                        {

                            bool[] id_ = new bool[4];
                            for (int i = 262; i < 266; i++)
                            {
                                id_[i - 262] = answer[i];
                            }
                            id = boolstoLong(id_);
                            break;
                        }
                        else
                        {
                            bool[] result = new bool[64];
                            for (int i = 194; i < 258; i++)
                            {
                                result[i - 194] = answer[i];
                            }
                            long finalResult = boolstoLong(result);
                            Console.WriteLine("Wynik sumowania: " + finalResult);
                            bool[] id_ = new bool[4];
                            for (int i = 262; i < 266; i++)
                            {
                                id_[i - 262] = answer[i];
                            }
                            id = boolstoLong(id_);
                            break;
                        }

                    }
                    else if (answer[258] == false && answer[259] == false && answer[260] == true && answer[261] == true)
                    {
                        Console.WriteLine("Przynajmniej jeden z argumentow jest niepoprawny!");
                        break;
                    }
                    else if (answer[258] == false && answer[259] == false && answer[260] == false && answer[261] == true)
                    {
                        Console.WriteLine("Uzyskana wartosc wykracza poza zakres!");
                        break;
                    }
                }
            }
        } while (option != "2");

    }
    static void Main(string[] args)
    {
        UdpClient client = new UdpClient();
        IPAddress broadcast = IPAddress.Parse("192.168.0.255");
        IPEndPoint ep = new IPEndPoint(broadcast, 11000);
        string option;
        try
        {
            do
            {
                Console.WriteLine("==============================");
                Console.WriteLine("[1]  Wykonaj operacje artmetyczna na trzech numberch calkowitych");
                Console.WriteLine("[2]  Sumuj wiele liczb");
                Console.WriteLine("[0]  Wyjscie");
                Console.Write("Podaj numer operacji: ");
                option = Console.ReadLine();
                Console.WriteLine("==============================");
                switch (option)
                {
                    case "1": art(client, ep); break;
                    case "2": addition(client, ep); break;
                    case "0": break;
                }
            } while (option != "0");
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.ReadLine();
        }
        finally
        {
            client.Close();
        }
    }
}