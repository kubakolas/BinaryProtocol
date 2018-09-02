using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program
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
    static bool[] append(bool[] op, bool[] l1, bool[] l2, bool[] l3, bool[] odp, bool[] stat, bool[] iden, bool flag, bool flag2)
    {
        bool[] message = new bool[272];
        op.CopyTo(message, 0);
        l1.CopyTo(message, 2);
        l2.CopyTo(message, 66);
        l3.CopyTo(message, 130);
        odp.CopyTo(message, 194);
        stat.CopyTo(message, 258);
        iden.CopyTo(message, 262);
        message[266] = flag;
        message[267] = flag2;
        for (int i = 268; i < 272; i++)
            message[i] = false;
        return message;
    }
    static byte[] boolstoBytes(bool[] bools)
    {
        int index = 0;
        int counter = 0;
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
            bits.CopyTo(Bytes, counter);
            counter += 1;
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
    private const int listenPort = 11000;
    static long identifier = 0;
    static long additionResult = 0;
    private static void StartListener()
    {

        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        
        Hashtable identifiers = new Hashtable();
        try
        {
            while (true)
            {
                Console.WriteLine("Oczekiwanie na odbior danych...");
                byte[] bytes = listener.Receive(ref groupEP);

                if(bytes.Length != 0)
                {
                    bool[] message = new bool[272]; //caly otrzymany message
                    message = bytesToBools(bytes);

                    bool[] id_klienta = new bool[4];
                         for (int i = 262; i < 266; i++)
                    {
                        id_klienta[i-262] = message[i];
                    }
                    long id_client = boolstoLong(id_klienta);
                    //ustawienie identifiera klientowi
                    if (id_client == 0)
                    {
                        identifier++;
                        bool[] id = longTobits(identifier);
                        for (int i = 262; i < 266; i++)
                        {
                            message[i] = id[i - 202];
                        }
                        identifiers.Add(identifier, identifiers);
                        id_client = identifier;
                    }
                    Console.WriteLine("Otrzymano dane od klienta o identifierze: " + id_client + " i adresie: " + groupEP.ToString());
                    //operacje artmetyczne
                    if (message[267] == false)
                    {
                        int operationNumber;
                        if (message[0] == false)
                        {
                            if (message[1] == false)
                            {
                                Console.Write("Mnozenie liczb: ");
                                operationNumber = 0;
                            }
                            else
                            {
                                Console.Write("Dodawanie liczb: ");
                                operationNumber = 1;
                            }
                        }
                        else
                        {
                            if (message[1] == false) { Console.Write("Srednia artmetyczna liczb: "); operationNumber = 3; }
                            else { Console.Write("Srednia geometryczna liczb: "); operationNumber = 4; }
                        }
                        //number 1
                        bool[] number1 = new bool[64];
                        for(int i =2; i < 66; i++)
                        {
                            number1[i - 2] = message[i];
                        }
                        long l1 = boolstoLong(number1);
                        Console.Write(l1 + " oraz ");
                        //number 2
                        bool[] number2 = new bool[64];
                        for (int i = 66; i < 130; i++)
                        {
                            number2[i - 66] = message[i];
                        }
                        long l2 = boolstoLong(number2);
                        Console.Write(l2 + " oraz ");
                        //number 3
                        bool[] number3 = new bool[64];
                        for (int i = 130; i < 194; i++)
                        {
                            number3[i - 130] = message[i];
                        }
                        long l3 = boolstoLong(number3);
                        Console.WriteLine(l3);
                        //wykonanie operacji na numberch
                        long result = 0;
                        if (operationNumber == 0)
                        {
                            result = l1*l2*l3;
                        }
                        else if (operationNumber == 1)
                        {
                            result = l2+l1+l3;
                        }
                        else if (operationNumber == 3)
                        {
                            result = (l1 + l2 + l3) / 3;
                        }
                        else if (operationNumber == 4)
                        {
                            result = (l1 * l2 * l3) / 3;
                        }
                        Console.WriteLine("result dzialania: " + result);
                        //ustawienie statusu
                        if (result > 9223372036854775807) //przekroczenie zakresu
                        {
                            message[258] = false; message[259] = false; message[260] = false; message[261] = true;
                        }
                        if(l1 < 0 || l2 < 0 || l3 < 0) //niepoprawne argumenty
                        {
                            message[258] = false; message[259] = false; message[260] = true; message[261] = true;
                        }

                        //zapisanie odpowiedzi w komunikacie
                        bool[] wyn = new bool[64];
                        wyn = longTobits(result);
                        for(int i = 194; i < 258; i++)
                        {
                            message[i] = wyn[i - 194];
                        }
                        //odeslanie messageu do klienta
                        bytes = boolstoBytes(message);
                        listener.Send(bytes, bytes.Length, groupEP);
                    }
                    //sumowanie
                    else
                    {
                        //jesli nie ma flagi z koncem sumowania to dodawaj 
                        if (message[266] == false)
                        {
                            bool[] l = new bool[64];
                            for (int i = 2; i < 66; i++)
                            {
                                l[i - 2] = message[i];
                            }
                            long received = boolstoLong(l);
                            identifiers[id_client] = (long)identifiers[id_client] + received;
                            Console.WriteLine("Otrzymano liczbe: " + received);
                            if (received < 0) //niepoprawne argumenty
                            {
                                message[258] = false; message[259] = false; message[260] = true; message[261] = true;
                            }
                            //odeslanie messageu do klienta
                            bytes = boolstoBytes(message);
                            listener.Send(bytes, bytes.Length, groupEP);
                        }
                        //jesli jest flaga konca sumowania to odeslij result
                        else
                        {
                            if ((long)identifiers[id_client] > 9223372036854775807) //przekroczenie zakresu
                            {
                                message[258] = false; message[259] = false; message[260] = false; message[261] = true;
                            }
                            Console.WriteLine("result sumowania: " + identifiers[id_client]);
                            //zapisanie odpowiedzi w komunikacie
                            bool[] wyn = new bool[64];
                            wyn = longTobits((long)identifiers[id_client]);
                            for (int i = 194; i < 258; i++)
                            {
                                message[i] = wyn[i - 194];
                            }
                            //odeslanie messageu do klienta
                            bytes = boolstoBytes(message);
                            listener.Send(bytes, bytes.Length, groupEP);
                        }
                    }
                }
               
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            listener.Close();
        }
    }

    public static int Main()
    {
       StartListener();
        Console.ReadLine();
        return 0;
    }
}