import { useEffect, useState } from "react";
import api from "../../services/api";
 
const LeaveHistory = () => {
  const [leaves, setLeaves] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filter, setFilter] = useState("ALL"); // ALL, PENDING, APPROVED, REJECTED
 
  useEffect(() => {
    loadHistory();
  }, []);
 
  const loadHistory = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await api.getMyLeaves();
      setLeaves(data);
    } catch (err) {
      setError(err.message || "Failed to load leave history");
    } finally {
      setLoading(false);
    }
  };
 
  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  };
 
  const calculateDays = (startDate, endDate) => {
    const start = new Date(startDate);
    const end = new Date(endDate);
    const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24)) + 1;
    return days;
  };
 
  const getStatusBadge = (status) => {
    const badges = {
      PENDING: "bg-warning text-dark",
      APPROVED: "bg-success",
      REJECTED: "bg-danger",
    };
    return badges[status] || "bg-secondary";
  };
 
  const filteredLeaves =
    filter === "ALL"
      ? leaves
      : leaves.filter((leave) => leave.status === filter);
 
  if (loading) {
    return (
<div className="card p-4">
<h5>Leave History</h5>
<div className="text-center py-3">
<div className="spinner-border" role="status">
<span className="visually-hidden">Loading...</span>
</div>
</div>
</div>
    );
  }
 
  return (
<div className="card p-4 mt-3">
<div className="d-flex justify-content-between align-items-center mb-3">
<h5 className="mb-0">Leave History</h5>
<button
          className="btn btn-sm btn-outline-primary"
          onClick={loadHistory}
          disabled={loading}
>
<i className="bi bi-arrow-clockwise"></i> Refresh
</button>
</div>
 
      {error && (
<div className="alert alert-danger alert-dismissible" role="alert">
          {error}
<button
            type="button"
            className="btn-close"
            onClick={() => setError(null)}
></button>
</div>
      )}
 
      {/* Filter Buttons */}
<div className="btn-group mb-3" role="group">
<button
          type="button"
          className={`btn btn-sm ${
            filter === "ALL" ? "btn-primary" : "btn-outline-primary"
          }`}
          onClick={() => setFilter("ALL")}
>
          All ({leaves.length})
</button>
<button
          type="button"
          className={`btn btn-sm ${
            filter === "PENDING" ? "btn-warning" : "btn-outline-warning"
          }`}
          onClick={() => setFilter("PENDING")}
>
          Pending ({leaves.filter((l) => l.status === "PENDING").length})
</button>
<button
          type="button"
          className={`btn btn-sm ${
            filter === "APPROVED" ? "btn-success" : "btn-outline-success"
          }`}
          onClick={() => setFilter("APPROVED")}
>
          Approved ({leaves.filter((l) => l.status === "APPROVED").length})
</button>
<button
          type="button"
          className={`btn btn-sm ${
            filter === "REJECTED" ? "btn-danger" : "btn-outline-danger"
          }`}
          onClick={() => setFilter("REJECTED")}
>
          Rejected ({leaves.filter((l) => l.status === "REJECTED").length})
</button>
</div>
 
      {filteredLeaves.length === 0 ? (
<div className="alert alert-info" role="alert">
          {filter === "ALL"
            ? "No leave requests found"
            : `No ${filter.toLowerCase()} leave requests`}
</div>
      ) : (
<div className="table-responsive">
<table className="table table-hover">
<thead>
<tr>
<th>Leave Type</th>
<th>Start Date</th>
<th>End Date</th>
<th>Days</th>
<th>Status</th>
</tr>
</thead>
<tbody>
              {filteredLeaves.map((leave) => (
<tr key={leave.leaveRequestId}>
<td>
<span className="badge bg-secondary">
                      {leave.leaveType}
</span>
</td>
<td>{formatDate(leave.startDate)}</td>
<td>{formatDate(leave.endDate)}</td>
<td>
                    {calculateDays(leave.startDate, leave.endDate)} days
</td>
<td>
<span className={`badge ${getStatusBadge(leave.status)}`}>
                      {leave.status}
</span>
</td>
</tr>
              ))}
</tbody>
</table>
</div>
      )}
 
      {filteredLeaves.length > 0 && (
<div className="text-muted small mt-2">
          Showing {filteredLeaves.length} of {leaves.length} leave requests
</div>
      )}
</div>
  );
};
 
export default LeaveHistory;