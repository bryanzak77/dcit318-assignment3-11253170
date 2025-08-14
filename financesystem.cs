using System;
using System.Collections.Generic;

public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing Bank Transfer of {transaction.Amount:C} for '{transaction.Category}'.");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing Mobile Money payment of {transaction.Amount:C} for '{transaction.Category}'.");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing Crypto Wallet transaction of {transaction.Amount:C} for '{transaction.Category}'.");
    }
}

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
        Console.WriteLine($"Applying transaction to Account {AccountNumber}.");
        Balance -= transaction.Amount;
    }
}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds.");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance for Savings Account {AccountNumber}: {Balance:C}");
        }
    }
}

public class FinanceApp
{
    private readonly List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        var savingsAccount = new SavingsAccount("S123456", 1000m);
        Console.WriteLine($"Initial Balance for Savings Account {savingsAccount.AccountNumber}: {1000m:C}\n");

        var transaction1 = new Transaction(1, DateTime.Now, 250m, "Groceries");
        var transaction2 = new Transaction(2, DateTime.Now, 150m, "Utilities");
        var transaction3 = new Transaction(3, DateTime.Now, 5000m, "Entertainment");

        ITransactionProcessor mobileMoneyProcessor = new MobileMoneyProcessor();
        ITransactionProcessor bankTransferProcessor = new BankTransferProcessor();
        ITransactionProcessor cryptoWalletProcessor = new CryptoWalletProcessor();

        Console.WriteLine("--- Processing Transactions ---");
        mobileMoneyProcessor.Process(transaction1);
        bankTransferProcessor.Process(transaction2);
        cryptoWalletProcessor.Process(transaction3);
        Console.WriteLine("-----------------------------");
        Console.WriteLine();

        Console.WriteLine("--- Applying Transactions to Savings Account ---");
        savingsAccount.ApplyTransaction(transaction1);
        savingsAccount.ApplyTransaction(transaction2);
        savingsAccount.ApplyTransaction(transaction3);
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine();

        _transactions.Add(transaction1);
        _transactions.Add(transaction2);
        _transactions.Add(transaction3);
    }
}

public class Program
{
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}