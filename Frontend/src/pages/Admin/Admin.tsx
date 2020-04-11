import React from "react";
import { RouteComponentProps, Redirect } from "react-router-dom";
import { useUser } from "../../providers/User";
import AdminSidebar from "../../components/Admin/AdminSidebar";
import AdminEditUser from "../../components/Admin/Users/AdminEditUser";
import AdminEditProject from "../../components/Admin/Projects/AdminEditProject";
import AdminListUsers from "../../components/Admin/Users/AdminListUsers";
import AdminListProjects from "../../components/Admin/Projects/AdminListProjects";

type TParams = {
  type: string;
  action: string;
  id: string | undefined;
}

const AdminPage = ({ match } : RouteComponentProps<TParams>): any => {
  const user = useUser();

  const adminInner = () => {
    if (match.params.action === "list") {
      if (match.params.type === "projects") {
        return <AdminListProjects />;
      } else if (match.params.type === "users") {
        return <AdminListUsers />;
      }
    } else if (match.params.action === "edit" && match.params.id !== undefined) {
      if (match.params.type === "projects") {
        return <AdminEditProject projectId={match.params.id} />;
      } else if (match.params.type === "users") {
        return <AdminEditUser userId={match.params.id} />;
      }
    }
    return <Redirect to="/404" />;
  };

  return <AdminSidebar>{ user?.isAdmin ? adminInner() : <h1>Not allowed</h1> }</AdminSidebar>;
};

export default AdminPage;