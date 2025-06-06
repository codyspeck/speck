﻿namespace Speck.DurableMessaging.IntegrationTests.Mailbox;

public class TestObserver<TItem>
{
    private readonly List<TItem> _items = [];
    
    public IEnumerable<TItem> Items => _items;
    
    public void Add(TItem item) => _items.Add(item);
}