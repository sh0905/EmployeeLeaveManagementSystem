
import LeaveApply from "../components/leave/LeaveApply";
import LeaveBalance from "../components/leave/LeaveBalance";
import LeaveHistory from "../components/leave/LeaveHistory";
 
export default () => (
<div className="container mt-4">
<div className="row">
<div className="col-md-4">
<LeaveBalance />
</div>
<div className="col-md-8">
<LeaveApply />
</div>
</div>
<div className="row mt-4">
<div className="col-12">
<LeaveHistory />
</div>
</div>
</div>
);
