import * as React from "react";
import ProjectList from "./ProjectList";
import { IProjectsProps, IProjectsState, Projects} from "../global/Projects";
import { ProjectFilter, IProjectFilter } from "./ProjectFilter";
import renderComponentIf from "../global/renderComponentIf";

interface IFindProjectProps extends IProjectsProps {
  controller: boolean;
  projectListContainerElement?: any;
  projectId?: number;
}

interface IFindProjectState extends IProjectsState {
  projectFilterState: IProjectFilter;
}

export default class FindProject extends Projects<IFindProjectProps, IFindProjectState> {

  // Define project list DOM element reference for scrolling
  private projectListContainerElement: React.RefObject<HTMLDivElement>;

  // Define number of projects we want to fetch on scroll
  private fetchNumberOfProjectsOnScroll;

  constructor (props) {
    super(props);
    const projectList = [];

    this.state = {
      projectList,
      projectFilterState: null,
      projectFetching: false,
      projectFetchError: null,
      allProjectsFetched: false
    };

    // Set constants
    this.projectListContainerElement = React.createRef();

    this.fetchNumberOfProjectsOnScroll = 15;
  }

  componentDidMount() {
    if (this.props.controller === false) {
      this.fetchProjects();
      window.addEventListener("scroll", this.fetchProjectsOnScroll, false);
    }
  }

  componentWillUnmount() {
    window.removeEventListener("scroll", this.fetchProjectsOnScroll, false);
  }

  fetchProjectsOnScroll = () => {
    let treshold = 200;

    // Check if window scrolled to half of project list
    if (window.pageYOffset + window.innerHeight >= (this.projectListContainerElement.current.offsetTop + this.projectListContainerElement.current.offsetHeight) - treshold) {

      // Fetch more projects
      this.fetchProjects(this.state.projectList.length);
    }
  }

  async fetchProjects(start: number = 0) {
    // Prevent unnecessary fetching
    if (this.state.projectFetching || this.state.allProjectsFetched) {
      return;
    }

    this.setState({ projectFetching: true });

      // Fetch projects with out filters set
    const getUrl = this.props.projectId == null ? `/api/projects/find?start=${start}&limit=${start + this.fetchNumberOfProjectsOnScroll}`
                                                : `/api/projects/${this.props.projectId}`;

    try {
      const searchProjectRequest: Request = new Request(getUrl);
      const fetchResult: Response = await fetch(searchProjectRequest);
      const jsonResponse = await fetchResult.json();

      // Add projects to project list
      this.setState((prevState) => ({
        projectFetching: false,
        projectList: [...prevState.projectList, ...jsonResponse]
      }));

      // We assume that if no or less than limit are returned, all projects have been fetched
      if (!jsonResponse.length || jsonResponse.length < this.fetchNumberOfProjectsOnScroll) {
        this.setState({ allProjectsFetched: true });
      }
    }
    catch (e) {
      this.setState({ projectFetching: false, projectFetchError: e });
    }
  }

  async fetchFilteredProjects(projectFilter: IProjectFilter = null) {
    this.setState({ projectFetching: true });

    // Fetch projects with filter
    const getUrl = `/api/projects/find?categoryId=${projectFilter.categoryId}&statusId=${projectFilter.statusId}`;

    try {
      const searchProjectRequest: Request = new Request(getUrl);
      const fetchResult: Response = await fetch(searchProjectRequest);
      const jsonResponse = await fetchResult.json();

      // Show filtered projects list
      this.setState(() => ({
        projectFetching: false,
        projectList: jsonResponse
      }));
    }
    catch (e) {
      this.setState({ projectFetching: false, projectFetchError: e });
    }
  }

  projectsUrl() {
    if (this.state.projectFilterState) {
      return `/api/projects/find?categoryId=${this.state.projectFilterState.categoryId}&statusId=${this.state.projectFilterState.statusId}`;
    }
    return "/api/projects/find";
  }

  onChange (newState: IProjectFilter) {
    this.setState(
      { projectFilterState: newState },
      () => this.fetchFilteredProjects(newState));
  }

  render() {
    const controller = <ProjectFilter onChange={(searchState: IProjectFilter) => this.onChange(searchState) }/>;
    return (
      <div id="find-project">
        { this.props.controller ?  controller : null }
        <div className="container" ref={this.projectListContainerElement}>
          <ProjectList projectList={this.state.projectList} tileClassName="col-xs-12 col-md-4" />
        </div>
      </div>
    );
  }
}

renderComponentIf(
  <FindProject controller={true} />,
  document.getElementById("project-controller")
);

renderComponentIf(
  <FindProject controller={false} />,
  document.getElementById("projects-container")
);

let embeddedProject = document.getElementById("embedded-project");
if (embeddedProject !== null) {
  let projectId = parseInt(embeddedProject.getAttribute("data-project-id"));
  renderComponentIf(
      <FindProject controller={false} projectId={projectId} />,
      document.getElementById("embedded-project")
  );
}