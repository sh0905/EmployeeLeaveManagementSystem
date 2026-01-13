import { useEffect, useState } from "react";
import api from "../../services/api";

const LeaveApproval = () => {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [processingId, setProcessingId] = useState(null);

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await api.getPendingLeaves();
      setRequests(data);
    } catch (err) {
      setError(err.message || "Failed to load pending requests");
    } finally {
      setLoading(false);
    }
  };

  const updateStatus = async (id, status) => {
    try {
      setProcessingId(id);
      setError(null);
      await api.updateLeaveStatus(id, status);
      // Reload the list after successful update
      await loadRequests();
    } catch (err) {
      setError(err.message || `Failed to ${status.toLowerCase()} leave`);
    } finally {
      setProcessingId(null);
    }
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  if (loading) {
    return (
      <div className="card p-4">
        <h5>Pending Leave Requests</h5>
        <div className="text-center py-3">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="card p-4">
      <h5>Pending Leave Requests</h5>

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

      {requests.length === 0 ? (
        <div className="alert alert-info" role="alert">
          No pending leave requests
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-hover">
            <thead>
              <tr>
                <th>Leave Type</th>
                <th>Start Date</th>
                <th>End Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {requests.map((request) => (
                <tr key={request.leaveRequestId}>
                  <td>
                    <span className="badge bg-secondary">
                      {request.leaveType}
                    </span>
                  </td>
                  <td>{formatDate(request.startDate)}</td>
                  <td>{formatDate(request.endDate)}</td>
                  <td>
                    <span className="badge bg-warning text-dark">
                      {request.status}
                    </span>
                  </td>
                  <td>
                    <button
                      className="btn btn-success btn-sm me-2"
                      onClick={() =>
                        updateStatus(request.leaveRequestId, "APPROVED")
                      }
                      disabled={processingId === request.leaveRequestId}
                    >
                      {processingId === request.leaveRequestId
                        ? "Processing..."
                        : "Approve"}
                    </button>
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() =>
                        updateStatus(request.leaveRequestId, "REJECTED")
                      }
                      disabled={processingId === request.leaveRequestId}
                    >
                      {processingId === request.leaveRequestId
                        ? "Processing..."
                        : "Reject"}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <button
        className="btn btn-sm btn-outline-primary mt-3"
        onClick={loadRequests}
        disabled={loading}
      >
        Refresh
      </button>
    </div>
  );
};

export default LeaveApproval;
