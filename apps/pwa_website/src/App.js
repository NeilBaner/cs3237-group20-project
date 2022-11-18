import React, { useEffect, useState } from "react";
import logo from "./logo.svg";
import "./App.css";
import useMediaQuery from "./hooks/useMediaQuery";
import Modal from "react-modal";

const customStyles = {
  content: {
    top: "50%",
    left: "50%",
    right: "auto",
    bottom: "auto",
    marginRight: "-50%",
    transform: "translate(-50%, -50%)",
    backgroundColor: "white",
    width: 400,
    maxWidth: "90vw"
  },
};

function App() {
    const isDesktop = useMediaQuery("(min-width: 1024px)");
    const [modalOpen, setModalOpen] = useState(false); 
    const [modalContent, setModalContent] = useState({
        bicep_curl: 31,
        shoulder_press: 30,
        left_shoulder_lateral_raise: 34,
        right_shoulder_lateral_raise : 36,
        shoulder_front_raise : 130,
        left_hand_tricep_extension : 70,
        right_hand_tricep_extension : 33,
        forward_lunge : 40,
        dumbbell_squat : 20,
    });

    // Query the Azure database
    /*useEffect(() => {
        async function fetchData() {
            console.log('fetching data');
            try {
                const response = await fetch('URL');
                data = await response.json();
                setModalContent(data);
            }
            catch (error) {
                data = "error fetching data"
            }
            console.log(data);
            console.log(typeof(data));
            console.log(data.address)
            setModalContent(data);

            Object.entries(modalContent).forEach(([key, value]) => console.log(`${key}: ${value}`));
        }

        fetchData();

    }, []); */

    function reformat(key) {
        var splitStr = key.toLowerCase().split("_");
        var reformatted = "";

        for (var i = 0; i < splitStr.length; i++) {
            splitStr[i] = splitStr[i].charAt(0).toUpperCase() + splitStr[i].substring(1)
            reformatted = splitStr.join(" ");
        }
      
        return reformatted;
    }

    return (
        <div className="App">
            <header className="App-header">
                <h1>BOYD<img src={logo} className="App-logo" alt="logo" /></h1>
            </header>
            {isDesktop ? (
                <div className="row display-container">
                    <div className="col">
                        <table className="table">
                            <tbody>
                                <tr>
                                    <td>
                                        <span className="material-icons">fitness_center</span>
                                    </td>
                                    <td>Exercised today</td>
                                </tr>
                                <tr>
                                    <td>
                                        <span className="material-icons">elderly</span>
                                    </td>
                                    <td>Active</td>
                                </tr>
                            </tbody>
                        </table>
                        <button onClick={() => setModalOpen(true)}>View past data</button>
                        <Modal
                            isOpen={modalOpen}
                            onRequestClose={() => setModalOpen(false)}
                            style={customStyles}
                        >
                            <h3>Exercise History (in seconds): </h3>
                            {Object.keys(modalContent).map((item, i) => (
                            <div key={i}> {reformat(item)}: {modalContent[item]} </div>))}
                            <button className="corner" onClick={() => setModalOpen(false)}>Close Modal</button>
                        </Modal>
                    </div>
                    <div className="row info-card">
                        <iframe width="560" height="315" src="https://www.youtube.com/embed/Wa8Fk8TaXPk" title="YouTube video player" frameBorder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowFullScreen></iframe>
                    </div> 
                </div>
            ) : (
                <div className="col">
                    <div className="row info-card">
                        <div className="col">
                        <iframe width="100%" height="100%" src="https://www.youtube.com/embed/Wa8Fk8TaXPk" title="YouTube video player" frameBorder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowFullScreen></iframe>
                        </div> 
                    </div>
                    <br/>
                    <div className="col">
                        <table className="table">
                            <tbody>
                                <tr>
                                    <td>
                                        <span className="material-icons">fitness_center</span>
                                    </td>
                                    <td>Exercised today</td>
                                </tr>
                                <tr>
                                    <td>
                                        <span className="material-icons">elderly</span>
                                    </td>
                                    <td>Active</td>
                                </tr>
                            </tbody>
                        </table>
                       <button onClick={() => setModalOpen(true)}>View past data</button>
                       <Modal
                            isOpen={modalOpen}
                            onRequestClose={() => setModalOpen(false)}
                            style={customStyles}
                        >
                            <h3>Exercise History (in seconds): </h3>
                            {Object.keys(modalContent).map((item, i) => (
                            <div key={item}> {reformat(item)}: {modalContent[item]} </div>))}
                            <button className="corner" onClick={() => setModalOpen(false)}>Close Modal</button>
                        </Modal>
                    </div>
                </div>
            )}
        </div>
    );
}

export default App;
