import React from "react";
import { RouteComponentProps, Redirect } from "react-router-dom";
import { UserContext } from "../../providers/user";
import AdminSidebar from "../../components/Admin/AdminSidebar";
import AdminEditUser from "../../components/Admin/Users/AdminEditUser";
import AdminEditProject from "../../components/Admin/Projects/AdminEditProject";
import AdminListUsers from "../../components/Admin/Users/AdminListUsers";
import AdminListProjects from "../../components/Admin/Projects/AdminListProjects";

type TParams = {
  type: string;
  action: string | undefined;
  id: string | undefined;
}

const AdminPageInner = ({ match } : RouteComponentProps<TParams>): any => {
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

const AdminPage = (props : RouteComponentProps<TParams>): any => 
  <AdminSidebar><UserContext.Consumer>{ ({user}) => user?.isAdmin ? <AdminPageInner history={props.history} location={props.location} match={props.match} /> : <h1>Not allowed</h1> }</UserContext.Consumer></AdminSidebar>;

export default AdminPage;