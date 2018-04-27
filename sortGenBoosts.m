filename = 'GenerationBoosts.txt';
fileID = fopen(filename,'r');
data = textscan(fileID, '%s%[^\n\r]', 'Delimiter', '', 'TextType', 'string',  'ReturnOnError', false);
fclose(fileID);

% remove the last empty column
data = data{1};

data_unique = unique(data);

if numel(data_unique)~=numel(data)
    msgbox('None unique data found');
    cbdata = [];
    for i = 1:numel(data_unique)
        cbdata = sprintf('%s%s\r\n', cbdata, data_unique{i});
    end
    clipboard('copy', cbdata);
else
    msgbox('Data is ok')
end