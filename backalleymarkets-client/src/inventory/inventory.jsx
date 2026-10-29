import React, { useState, useEffect } from "react";
import "./inventory.css";
import { useParams, Link, useNavigate } from "react-router";
import marketImage from "../assets/images/kayle-kaupanger-J8ksCswaBYo-unsplash.jpg";

function Inventory() {
  const [items, setItems] = useState([]);

  //   const itemToPass = {
  //     itemName: '',
  //     itemId: null
  //   }

  let { itemId } = useParams();

  let navigate = useNavigate();

  const addItem = () => {
    navigate("/addItem");
  };

  useEffect(() => {
    const initGetItems = async () => {
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
    };

    initGetItems();
  }, []);

  async function getItems() {
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
  }

  return (
    <>
      <div className="card grid gap-4">
        <div className="grid grid-cols-2 gap-4">
          <button className="bg-sky-500" onClick={() => getItems()}>
            Click for Items
          </button>
          <button
            className="bg-indigo-500 hover:bg-fuchsia-500"
            onClick={addItem}
          >
            Add New Item
          </button>
        </div>
        <div className="grid gap-4">
          {items.map((item) => (
            <div key={item.itemId}>
              <Link
                key={item.itemId}
                to={"/items/" + item.itemId}
                state={item}
                className="grid grid-cols-3 grid-rows-3 gap-4 border-2 border-stone-500 p-2 rounded-md shadow-md"
              >
                <img
                  src={marketImage}
                  alt="Open market with stalls."
                  className="row-span-3"
                />
                <div className="col-span-2 text-left">{item.itemName}</div>
                <div className="col-span-2 row-span-2 text-left">{item.description}</div>
              </Link>
            </div>
          ))}
        </div>
      </div>
    </>
  );
}

export default Inventory;
