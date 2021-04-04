using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Inventories.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Inventories.Services
{

    public interface IInventoryService
    {
        (bool result, string message) Add(string DeclarationNumber,int Quantity,string ProductCode);
        (bool result, string message) Remmove(string DeclarationNumber, int Quantity,string ProductCode);

        (List<DeClarationDetail> result , string message) AddList(List<DeClarationDetail> deClarationDetails);
        (List<DeClarationDetail> errors, string message) RemmoveList(List<DeClarationDetail> deClarationDetails, string ParentNumber);

        (Inventory inventorie, string message) GetInventory(string DeclarationNumber, string ProductCode);
        (List<Inventory> inventories, string message) GetInventoyByDeclarationNumber(string DeclarationNumber);
        (List<Inventory> inventories, string message) GetInventoyByProductCode(string ProductCode);
    }

    public class InventoryService : IInventoryService
    {

        private readonly MariaDBContext _context;

        public InventoryService(MariaDBContext context)
        {
            _context = context;
        }

        public (List<DeClarationDetail> result, string message) AddList(List<DeClarationDetail> deClarationDetails)
        {
            List<DeClarationDetail> reslutClarationDetails = new List<DeClarationDetail>();
            foreach (DeClarationDetail clarationDetail in deClarationDetails)
            {
                Inventory checkInventory = _context.Inventories.FirstOrDefault(x => x.DeNumber == clarationDetail.DeClaNumber && x.ProductCode == clarationDetail.ProductCode);
                if (checkInventory == null)
                {
                    Inventory newInventory = new Inventory
                    {
                        DeNumber = clarationDetail.DeClaNumber,
                        InQuantity = clarationDetail.DeClaDetailQuantity,
                        ProductCode = clarationDetail.ProductCode
                    };
                    if(clarationDetail.DeClaDetailQuantity == 0)
                    {
                        newInventory.SettlementDate = DateTime.UtcNow.AddHours(7);
                    }
                    _context.Inventories.Add(newInventory);
                    _context.SaveChanges();
                    reslutClarationDetails.Add(clarationDetail);
                }
                else
                {
                    checkInventory.InQuantity = checkInventory.InQuantity + clarationDetail.DeClaDetailQuantity;
                    if (checkInventory.InQuantity == 0)
                    {
                        checkInventory.SettlementDate = DateTime.UtcNow.AddHours(7);
                    }
                    _context.SaveChanges();
                    reslutClarationDetails.Add(clarationDetail);
                }
            }
            return (reslutClarationDetails, "Success");
           
           
        }
        public (List<DeClarationDetail> errors, string message) RemmoveList(List<DeClarationDetail> deClarationDetails, string ParentNumber)
        {
            List<DeClarationDetail> erroClarationDetails = new List<DeClarationDetail>();
            foreach (DeClarationDetail clarationDetail in deClarationDetails)
            {
                Inventory checkInventory = _context.Inventories.FirstOrDefault(x => x.DeNumber == ParentNumber && x.ProductCode == clarationDetail.ProductCode && x.InQuantity >= clarationDetail.DeClaDetailQuantity);
                if (checkInventory == null)
                {
                    erroClarationDetails.Add(clarationDetail);
                    break;
                }
                if (checkInventory.InQuantity < clarationDetail.DeClaDetailQuantity)
                {
                    erroClarationDetails.Add(clarationDetail);
                    break;
                }
                checkInventory.InQuantity = checkInventory.InQuantity - clarationDetail.DeClaDetailQuantity;

                if (checkInventory.InQuantity <= 0)
                {
                    checkInventory.InQuantity = 0;
                    checkInventory.SettlementDate = DateTime.UtcNow.AddHours(7);
                }
                    
            }
            _context.SaveChanges();
            return (erroClarationDetails , "Success");
        }

        public (Inventory inventorie, string message) GetInventory(string DeclarationNumber, string ProductCode)
        {
            Inventory inventory = _context.Inventories.FirstOrDefault(x => x.DeNumber == DeclarationNumber && x.ProductCode == ProductCode);
            return (inventory, "Success");
        }

        public (List<Inventory> inventories, string message) GetInventoyByDeclarationNumber(string DeclarationNumber)
        {
            List<Inventory> inventories = _context.Inventories.Where(x => x.DeNumber == DeclarationNumber).ToList();
            return (inventories, "Success");
        }

        public (List<Inventory> inventories, string message) GetInventoyByProductCode(string ProductCode)
        {
            List<Inventory> inventories = _context.Inventories.Where(x => x.ProductCode == ProductCode).ToList();
            return (inventories, "Success");
        }

        public (bool result, string message) Remmove(string DeclarationNumber, int Quantity, string ProductCode)
        {
            if (Quantity < 0) return (false, "The number must not be negative");
            Inventory checkInventory = _context.Inventories.FirstOrDefault(x => x.DeNumber == DeclarationNumber && x.ProductCode == ProductCode);
            if (checkInventory == null) return (false, "Inventory not found!");
            checkInventory.InQuantity = checkInventory.InQuantity - Quantity;
            if (checkInventory.InQuantity <= 0)
            {
                checkInventory.InQuantity = 0;
                checkInventory.SettlementDate = DateTime.UtcNow.AddHours(7);
            }
                
            _context.SaveChanges();
            return (true, "Remove inventory success!");
        }

        public (bool result, string message) Add(string DeclarationNumber, int Quantity, string ProductCode)
        {
            if (Quantity < 0) return (false, "The number must not be negative");
            Inventory checkInventory = _context.Inventories.FirstOrDefault(x=> x.DeNumber == DeclarationNumber && x.ProductCode == ProductCode);
            if(checkInventory == null)
            {
                Inventory newInventory = new Inventory
                {
                    DeNumber = DeclarationNumber,
                    InQuantity = Quantity,
                    ProductCode = ProductCode
                };
                _context.Inventories.Add(newInventory);
                _context.SaveChanges();
                return (true, "Created inventory success!");
            }
            checkInventory.InQuantity = checkInventory.InQuantity + Quantity;
            if (checkInventory.InQuantity <= 0)
            {
                checkInventory.InQuantity = 0;
                checkInventory.SettlementDate = DateTime.UtcNow.AddHours(7);
            }
            _context.SaveChanges();
            return (true, "Created inventory success!");
        }

    }
}
