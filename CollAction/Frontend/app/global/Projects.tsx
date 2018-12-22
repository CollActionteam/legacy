import * as React from "react";
import { IProject } from "../project/ProjectList";

export interface IProjectsProps {
}

export interface IProjectsState {
  projectList: IProject[];
  projectFetching: boolean;
  projectFetchError: any;
}

export abstract class Projects<P extends IProjectsProps, S extends IProjectsState> extends React.Component<P, S> {

    abstract projectsUrl(): string;

    async fetchProjects() {
        this.setState({ projectFetching: true });

        try {
            const searchProjectRequest: Request = new Request(this.projectsUrl());

            const verificationToken = document.getElementById("RequestVerificationToken") as HTMLInputElement;
            if (verificationToken) {
                searchProjectRequest.headers.set("RequestVerificationToken", verificationToken.value);
            }

            const fetchResult: Response = await fetch(searchProjectRequest);
            const jsonResponse = await fetchResult.json();
            this.setState({ projectFetching: false, projectList: jsonResponse });
        }
        catch (e) {
            this.setState({ projectFetching: false, projectFetchError: e });
        }
    }
}