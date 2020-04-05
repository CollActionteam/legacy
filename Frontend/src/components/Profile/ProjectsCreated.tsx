import React from "react";
import { IUser } from "../../api/types";
import { CardContent, Card } from "@material-ui/core";
import ProjectCreated from "./ProjectCreated";

interface IProjectsCreatedProps {
    user: IUser;
}

export default ({ user }: IProjectsCreatedProps) => {
    return <Card>
            <CardContent>
                <h3>Projects Created</h3>
                { 
                    user.projects.map(project => <ProjectCreated key={project.id} project={project} user={user} />)
                }
                { user.projects.length === 0 ? <p>You have no created projects</p> : null }
            </CardContent>
        </Card>;
};