import React from "react";
import { IUser } from "../../../api/types";
import { CardActions, CardContent, Card, Button } from "@material-ui/core";
import ProjectCard from "../../ProjectCard";

interface IProjectCreatedProps {
    user: IUser;
}

export default ({ user }: IProjectCreatedProps) => {
    return <Card>
            <CardContent>
                <h3>Projects Created</h3>
                { 
                    user.projects.map(project => <ProjectCard project={project} />)
                }
                { user.projects.length === 0 ? <p>You have no created projects</p> : null }
            </CardContent>
        </Card>;
};