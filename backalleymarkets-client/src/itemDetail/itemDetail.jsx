import React, { useState, useEffect } from "react";
import "./itemDetail.css";
import { useParams, useLocation, useNavigate } from "react-router";

const ItemDetail = () => {
  const location = useLocation();
  const receivedItem = location.state;

  let navigate = useNavigate();

  const navItemList = () => {
    navigate('/');
  }
  // const {itemName} = useParams();

  //   const [item, setItem] = useState("");

  //   useEffect(() => {
  //       const initGetItem = async () => {
  //         try {
  //           const res = await fetch("http://localhost:5142/api/Items/");
  //           const data = await res.json();

  //           setItem(data);
  //           console.log("Items after: ", items);
  //         } catch (error) {
  //           console.error("Error fetching data: ", error);
  //         }
  //       };

  //       initGetItems();
  //     }, []);

  return (
    <>
      <div className="card">
        <h3>{receivedItem.itemName}</h3>
        <button
          className="bg-indigo-500 hover:bg-fuchsia-500"
          onClick={navItemList}
        >
          Return to List
        </button>
      </div>
    </>
  );
};

export default ItemDetail;
