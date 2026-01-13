
import { Routes, Route } from "react-router-dom";
import Navbar from "./components/common/Navbar";
import ProtectedRoute from "./components/layout/ProtectedRoute";
import Home from "./pages/Home";
import ApplyLeave from "./pages/ApplyLeave";
import ManageLeaves from "./pages/ManageLeaves";

function App() {
  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
       <Route
  path="/employee"
  element={
    <ProtectedRoute role="EMPLOYEE">
      <ApplyLeave />
    </ProtectedRoute>
  }
/>

<Route
  path="/manager"
  element={
    <ProtectedRoute role="MANAGER">
      <ManageLeaves />
    </ProtectedRoute>
  }
/>

      </Routes>
    </>
  );
}

export default App;
