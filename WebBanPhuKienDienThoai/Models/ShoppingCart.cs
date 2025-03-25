namespace WebBanPhuKienDienThoai.Models
{
    public class ShoppingCart 
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int GetTotalQuantity()
        {
            return Items.Sum(i => i.Quantity);
        }
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId ==
           item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }
        public decimal GetTotalPrice()
        {
            return Items.Sum(i => i.Price * i.Quantity);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public void UpdateItem(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }


    }
}
