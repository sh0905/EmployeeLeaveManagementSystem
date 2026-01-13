import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

const Home = () => {
  const { login } = useContext(AuthContext);
  const navigate = useNavigate();

  const loginAsEmployee = () => {
    login("EMPLOYEE");
    navigate("/employee");
  };

  const loginAsManager = () => {
    login("MANAGER");
    navigate("/manager");
  };

  return (
    <div className="container mt-5">
      <h3>Select Role</h3>

      <button
        className="btn btn-primary me-2"
        onClick={loginAsEmployee}
      >
        Login as Employee
      </button>

      <button
        className="btn btn-secondary"
        onClick={loginAsManager}
      >
        Login as Manager
      </button>
    </div>
  );
};

export default Home;
