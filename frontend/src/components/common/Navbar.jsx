
import { useContext } from "react";
import { Link } from "react-router-dom";
import { AuthContext } from "../../context/AuthContext";

const Navbar = () => {
  const { user, logout } = useContext(AuthContext);

  return (
    <nav className="navbar navbar-dark bg-dark px-3">
      <span className="navbar-brand">Leave Management App</span>
      {user && (
        <div>
          <span className="text-light me-3">{user.role}</span>
          <button className="btn btn-sm btn-danger" onClick={logout}>Logout</button>
        </div>
      )}
    </nav>
  );
};

export default Navbar;
