import { useEffect, useState } from "react";
import api from "../../services/api";

const LeaveBalance = () => {
  const [balance, setBalance] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadBalance();
  }, []);

  const loadBalance = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await api.getLeaveBalance();
      setBalance(data);
    } catch (err) {
      setError(err.message || "Failed to load leave balance");
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="card p-4">
        <h5>Leave Balance</h5>
        <div className="text-center py-3">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="card p-4">
        <h5>Leave Balance</h5>
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
        <button className="btn btn-sm btn-primary" onClick={loadBalance}>
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className="card p-4">
      <h5>Leave Balance</h5>
      {balance && (
        <ul className="list-group">
          <li className="list-group-item d-flex justify-content-between align-items-center">
            Vacation
            <span className="badge bg-primary rounded-pill">
              {balance.vacation} days
            </span>
          </li>
          <li className="list-group-item d-flex justify-content-between align-items-center">
            Sick
            <span className="badge bg-info rounded-pill">
              {balance.sick} days
            </span>
          </li>
          <li className="list-group-item d-flex justify-content-between align-items-center">
            Casual
            <span className="badge bg-success rounded-pill">
              {balance.casual} days
            </span>
          </li>
        </ul>
      )}
    </div>
  );
};

export default LeaveBalance;
