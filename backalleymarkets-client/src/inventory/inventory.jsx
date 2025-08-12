import React, { useState, useEffect } from "react";
import "./inventory.css";

function Inventory() {
  const [items, setItems] = useState([]);

  useEffect(() => {
    const initGetItems = async () => {
      try {
        const res = await fetch("http://localhost:5142/api/Items/");
        const data = await res.json();
        console.log("Returnded data: ", data);
        console.log("Items before: ", items);

        setItems(data);
        console.log("Items after: ", items);
      } catch (error) {
        console.error("Error fetching data: ", error);
      }
    };

    initGetItems();
  }, []);

  async function getItems() {
    try {
      const res = await fetch("http://localhost:5142/api/Items/");
      const data = await res.json();
      console.log("Returnded data: ", data);
      console.log("Items before: ", items);

      setItems([]);
      console.log("Items after: ", items);
    } catch (error) {
      console.error("Error fetching data: ", error);
    }
  }

  return (
    <>
      <div className="card">
        <button
          className="bg-indigo-500 hover:bg-fuchsia-500"
          onClick={() => getItems()}
        >
          Click for Items
        </button>
        <div className="grid grid-cols-4 gap-4">
          {items.map((item) => (
            <a key={item.itemId} href={"/" + item.itemId}>
              <div key={item.itemId}>{item.itemName}</div>
            </a>
          ))}
        </div>
      </div>
    </>
  );
}

export default Inventory;
