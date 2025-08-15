using System;
using System.Collections.Generic;

// =============================
// 1. Transaction Record
// =============================
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// =============================
// 2. Interface for Processing Transactions
// =============================
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// =============================
// 3. Concrete Processors
// =============================
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer of {transaction.Amount:C} for {transaction.Category}.");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Sending mobile money: {transaction.Amount:C} for {transaction.Category}.");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Transferring crypto: {transaction.Amount:C} for {transaction.Category} (volatile, but processed!).");
    }
}

// =============================
// 4. Base Account Class
// =============================
public class Account
{
    public string AccountNumber { get; }
    protected decimal Balance { get; set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// =============================
// 5. Sealed SavingsAccount Class
// =============================
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine($"Insufficient funds: Cannot deduct {transaction.Amount:C} from balance of {Balance:C}.");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. Updated balance: {Balance:C}");
        }
    }
}

// =============================
// 6. FinanceApp Class
// =============================
public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        var account = new SavingsAccount("SAV-22011137", 1000.00m);

        var transaction1 = new Transaction(1, DateTime.Now, 150.00m, "Groceries");
        var transaction2 = new Transaction(2, DateTime.Now, 80.00m, "Utilities");
        var transaction3 = new Transaction(3, DateTime.Now, 200.00m, "Entertainment");

        var mobileProcessor = new MobileMoneyProcessor();
        var bankProcessor = new BankTransferProcessor();
        var cryptoProcessor = new CryptoWalletProcessor();

        mobileProcessor.Process(transaction1);
        bankProcessor.Process(transaction2);
        cryptoProcessor.Process(transaction3);

        Console.WriteLine("\n--- Applying Transactions to Account ---");
        account.ApplyTransaction(transaction1);
        account.ApplyTransaction(transaction2);
        account.ApplyTransaction(transaction3);

        _transactions.Add(transaction1);
        _transactions.Add(transaction2);
        _transactions.Add(transaction3);

        Console.WriteLine($"\nTotal transactions recorded: {_transactions.Count}");
    }
}

// =============================
// 7. Main Method
// =============================
class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}