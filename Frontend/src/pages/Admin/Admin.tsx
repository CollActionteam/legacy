import React from "react";
import { ManageProjects, EditProject } from "../../components/Admin/ManageProjects";
import { ManageUsers, EditUser } from "../../components/Admin/ManageUsers";
import { RouteComponentProps, Redirect } from "react-router-dom";
import { AdminSidebar } from "../../components/Admin/Sidebar";

type TParams = {
  type: string;
  action: string | undefined;
  id: string | undefined;
}

const AdminPageInner = ({ match } : RouteComponentProps<TParams>): any => {
  if (match.params.action === "list") {
    if (match.params.type === "projects") {
      return <ManageProjects />
    } else if (match.params.type === "users") {
      return <ManageUsers />
    }
  } else if (match.params.action === "edit" && match.params.id !== undefined) {
    if (match.params.type === "projects") {
      return <EditProject projectId={match.params.id} />
    } else if (match.params.type === "users") {
      return <EditUser userId={match.params.id} />
    }
  }

  return <Redirect to="/404" />
};

const AdminPage = (props : RouteComponentProps<TParams>): any => 
  <AdminSidebar><AdminPageInner history={props.history} location={props.location} match={props.match} /></AdminSidebar>;

export default AdminPage;