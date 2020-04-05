import { DiscussionEmbed } from 'disqus-react';
import { IProject } from '../../api/types';
import React from 'react';

export default ({ project }: { project: IProject }) => {
    const disqusId = `${project.name}/${project.id}`;
    const disqusSiteId = "www-collaction-org";
    return <DiscussionEmbed shortname={disqusSiteId} config={
    {
        url: window.location.href,
        identifier: disqusId,
        title: `${project.name} discussion`
    }} />;
};