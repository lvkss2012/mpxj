package net.sf.mpxj.junit;

import net.sf.mpxj.MPXJException;
import net.sf.mpxj.ProjectFile;
import net.sf.mpxj.primavera.PrimaveraXERFileReader;
import net.sf.mpxj.reader.UniversalProjectReader;
import net.sf.mpxj.Task;

import java.io.IOException;
import java.nio.charset.Charset;

public class TestMpp {
    public static void main(String[] args) throws MPXJException, IOException, IllegalAccessException, InstantiationException {
        final String filePath = "F:\\test\\test-mpxj\\test1.mpp";
        final String filePath1 = "G:\\工作\\项目总进度计划表1.mpp";
        final String filePath2 = "G:\\工作\\去污车间四级进度计划.xer";
        final String outFilePath = "F:\\test\\test-mpxj\\writerout.mpp";
        final String jsonFilePath = "F:\\test\\test-mpxj\\writerout.json";
        final String xmlFilePath = "F:\\test\\test-mpxj\\writerout1.xml";
        UniversalProjectReader reader1 = new UniversalProjectReader();
        ProjectFile project = reader1.read(filePath2);

//        PrimaveraXERFileReader reader = new PrimaveraXERFileReader();
//        reader.setCharset(Charset.forName("GB2312"));
//        ProjectFile project = reader.read(filePath2);

        for (Task task : project.getTasks()) {
            System.out.println("Task: " + task.getName() + " ID=" + task.getID() + " Unique guid=" + task.getGUID());
        }
    }
}
