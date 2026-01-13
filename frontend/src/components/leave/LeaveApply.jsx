import { useState } from "react";
import api from "../../services/api.js";

const LeaveApply = () => {
  const [form, setForm] = useState({
    startDate: "",
    endDate: "",
    leaveType: "VACATION",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSuccess(false);

    const start = new Date(form.startDate);
    const end = new Date(form.endDate);
    const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24)) + 1;

    if (days <= 0) {
      setError("Invalid date range. End date must be after start date.");
      return;
    }

    setLoading(true);

    try {
      await api.applyLeave({
        ...form,
        days,
      });
      setSuccess(true);
      // Reset form
      setForm({
        startDate: "",
        endDate: "",
        leaveType: "VACATION",
      });
      setTimeout(() => setSuccess(false), 3000);
    } catch (err) {
      setError(err.message || "Failed to apply leave. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card p-4 mt-3">
      <h5>Apply Leave</h5>

      {error && (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      )}

      {success && (
        <div className="alert alert-success" role="alert">
          Leave applied successfully!
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">Start Date</label>
          <input
            type="date"
            className="form-control"
            value={form.startDate}
            onChange={(e) =>
              setForm({ ...form, startDate: e.target.value })
            }
            required
            disabled={loading}
          />
        </div>

        <div className="mb-3">
          <label className="form-label">End Date</label>
          <input
            type="date"
            className="form-control"
            value={form.endDate}
            onChange={(e) =>
              setForm({ ...form, endDate: e.target.value })
            }
            required
            disabled={loading}
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Leave Type</label>
          <select
            className="form-control"
            value={form.leaveType}
            onChange={(e) =>
              setForm({ ...form, leaveType: e.target.value })
            }
            disabled={loading}
          >
            <option value="VACATION">Vacation</option>
            <option value="SICK">Sick</option>
            <option value="CASUAL">Casual</option>
          </select>
        </div>

        <button
          type="submit"
          className="btn btn-primary"
          disabled={loading}
        >
          {loading ? "Submitting..." : "Apply Leave"}
        </button>
      </form>
    </div>
  );
};

export default LeaveApply;
